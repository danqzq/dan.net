using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

namespace BeardedManStudios
{
	public sealed class MainThreadManager : MonoBehaviour, MainThreadManager.IThreadRunner
	{
		public enum UpdateType
		{
			FixedUpdate,
			Update,
			LateUpdate,
		}

		public delegate void UpdateEvent();
		public static UpdateEvent unityFixedUpdate = null;
		public static UpdateEvent unityUpdate = null;
		public static UpdateEvent unityLateUpdate = null;
		
		public static Action repeatedUpdate = null;
		public static float repeatedUpdateDelay = 0f;
		
		public static void RunRepeated(Action action, float delay)
		{
			repeatedUpdate = action;
			repeatedUpdateDelay = Time.time + delay;
			
			unityUpdate += () =>
			{
				if (Time.time >= repeatedUpdateDelay)
				{
					repeatedUpdate?.Invoke();
					repeatedUpdateDelay = Time.time + delay;
				}
			};
		}
		
		public static void RunDelayed(Action action, float delay, UpdateType updateType = UpdateType.Update)
		{
			if (delay <= 0f)
			{
				Run(action, updateType);
				return;
			}

			var startTime = Time.time + delay;
			Run(() =>
			{
				if (Time.time >= startTime)
				{
					action.Invoke();
				}
			}, updateType);
		}

		/// <summary>
		/// The singleton instance of the Main Thread Manager
		/// </summary>
		private static MainThreadManager _instance;
		public static MainThreadManager Instance
		{
			get
			{
				if (_instance == null)
					Create();

				return _instance;
			}
		}

		/// <summary>
		/// This will create a main thread manager if one is not already created
		/// </summary>
		public static void Create()
		{
			if (_instance != null)
				return;

			ThreadManagement.Initialize();

			if (!ReferenceEquals(_instance, null))
				return;

			new GameObject("Main Thread Manager").AddComponent<MainThreadManager>();
		}

		/// <summary>
		/// A dictionary of action queues for different updates.
		/// </summary>
		private static Dictionary<UpdateType, Queue<Action>> actionQueueDict = new Dictionary<UpdateType, Queue<Action>>();
		private static Dictionary<UpdateType, Queue<Action>> actionRunnerDict = new Dictionary<UpdateType, Queue<Action>>();

		// Setup the singleton in the Awake
		private void Awake()
		{
			// If an instance already exists then delete this copy
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}

			// Assign the static reference to this object
			_instance = this;

			// This object should move through scenes
			DontDestroyOnLoad(gameObject);
		}

		public void Execute(Action action)
		{
			Run(action);
		}

		/// <summary>
		/// Add a function to the list of functions to call on the main thread via the Update function
		/// </summary>
		/// <param name="action">The method that is to be run on the main thread</param>
		public static void Run(Action action, UpdateType updateType = UpdateType.FixedUpdate)
		{
			// Only create this object on the main thread
#if UNITY_WEBGL
			if (ReferenceEquals(Instance, null))
#else
			if (ReferenceEquals(Instance, null) && ThreadManagement.IsMainThread)
#endif
			{
				Create();
			}

			// Allocate new action queue by update type if there's no one exists.
			if (!actionQueueDict.ContainsKey(updateType))
			{
				actionQueueDict.Add(updateType, new Queue<Action>());

				// Since an action runner depends on the action queue, allocate new one here.
				actionRunnerDict.Add(updateType, new Queue<Action>());
			}

			Queue<Action> mainThreadActions = actionQueueDict[updateType];

			// Make sure to lock the mutex so that we don't override
			// other threads actions
			lock (mainThreadActions)
			{
				mainThreadActions.Enqueue(action);
			}
		}

		private void HandleActions(UpdateType updateType)
		{
			// Allocate new action queue by update type if there's no one exists.
			if (!actionQueueDict.ContainsKey(updateType))
			{
				actionQueueDict.Add(updateType, new Queue<Action>());

				// Since an action runner depends on the action queue, allocate new one here.
				actionRunnerDict.Add(updateType, new Queue<Action>());

			}
			Queue<Action> mainThreadActions = actionQueueDict[updateType];
			Queue<Action> mainThreadActionsRunner = actionRunnerDict[updateType];

			lock (mainThreadActions)
			{
				// Flush the list to unlock the thread as fast as possible
				if (mainThreadActions.Count > 0)
				{
					while (mainThreadActions.Count > 0)
						mainThreadActionsRunner.Enqueue(mainThreadActions.Dequeue());
				}
			}

			// If there are any functions in the list, then run
			// them all and then clear the list
			if (mainThreadActionsRunner.Count > 0)
			{
				while (mainThreadActionsRunner.Count > 0)
					mainThreadActionsRunner.Dequeue()();
			}
		}

		private void FixedUpdate()
		{
			HandleActions(UpdateType.FixedUpdate);

			if (unityFixedUpdate != null)
				unityFixedUpdate();
		}

		private void Update()
		{
			HandleActions(UpdateType.Update);

			if (unityUpdate != null)
				unityUpdate();
		}

		private void LateUpdate()
		{
			HandleActions(UpdateType.LateUpdate);

			if (unityLateUpdate != null)
				unityLateUpdate();
		}
		
		public interface IThreadRunner
		{
			void Execute(Action action);
		}

#if WINDOWS_UWP
		public static async void ThreadSleep(int length)
#else
		public static void ThreadSleep(int length)
#endif
		{
#if WINDOWS_UWP
			await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(length));
#else
			System.Threading.Thread.Sleep(length);
#endif
		}
		
		public static class ThreadManagement
		{
			public static int MainThreadId { get; private set; }

			public static int GetCurrentThreadId()
			{
				return Thread.CurrentThread.ManagedThreadId;
			}

			public static void Initialize() { MainThreadId = GetCurrentThreadId(); }

			public static bool IsMainThread
			{
				get { return GetCurrentThreadId() == MainThreadId; }
			}
		}

		/// <summary>
		/// A class for calling methods on a separate thread
		/// </summary>
		public static class Task
		{
			/// <summary>
			/// Sets the method that is to be executed on the separate thread
			/// </summary>
			/// <param name="expression">The method that is to be called on the newly created thread</param>
			private static void QueueExpression(WaitCallback expression)
			{
				ThreadPool.QueueUserWorkItem(expression);
			}

			/// <summary>
			/// Used to run a method / expression on a separate thread
			/// </summary>
			/// <param name="expression">The method to be run on the separate thread</param>
			/// <param name="delayOrSleep">The amount of time to wait before running the expression on the newly created thread</param>
			/// <returns></returns>
			public static void Queue(Action expression, int delayOrSleep = 0)
			{
				// Wrap the expression in a method so that we can apply the delayOrSleep before and remove the task after it finishes
				WaitCallback inline = (state) =>
				{
					// Apply the specified delay
					if (delayOrSleep > 0)
						Thread.Sleep(delayOrSleep);

					// Call the requested method
					expression();
				};

				// Set the method to be called on the separate thread to be the inline method we have just created
				QueueExpression(inline);
			}

#if WINDOWS_UWP
		public async static void Sleep(int milliseconds)
		{
			await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(milliseconds));
		}
#else
			public static void Sleep(int milliseconds)
			{
				Thread.Sleep(milliseconds);
			}
#endif
		}
	}
}