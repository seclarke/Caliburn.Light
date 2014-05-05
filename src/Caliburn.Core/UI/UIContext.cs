﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caliburn.Light
{
    /// <summary>
    /// Shared UI context
    /// </summary>
    public static class UIContext
    {
        private static int? _managedThreadId;
        private static TaskScheduler _taskScheduler;
        private static bool _isInDesignTool = true;
        private static IViewAdapter _viewAdapter;

        /// <summary>
        /// Initializes the <see cref="UIContext"/>.
        /// </summary>
        /// <param name="isInDesignTool">Whether or not the framework is running in the context of a designer.</param>
        /// <param name="viewAdapter">The adapter to interact with view elements.</param>
        public static void Initialize(bool isInDesignTool, IViewAdapter viewAdapter)
        {
            _managedThreadId = Environment.CurrentManagedThreadId;
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _isInDesignTool = isInDesignTool;
            _viewAdapter = viewAdapter;
        }

        private static IViewAdapter ViewAdapter
        {
            get { return _viewAdapter ?? NullViewAdapter.Instance; }
        }

        /// <summary>
        /// Indicates whether or not the framework is running in the context of a designer.
        /// </summary>
        public static bool IsInDesignTool
        {
            get { return _isInDesignTool; }
        }

        /// <summary>
        /// Determines whether the calling thread is the thread associated with the UI context. 
        /// </summary>
        /// <returns></returns>
        public static bool CheckAccess()
        {
            return !_managedThreadId.HasValue || _managedThreadId == Environment.CurrentManagedThreadId;
        }

        /// <summary>
        /// The <see cref="TaskScheduler"/> associated with the UI context.
        /// </summary>
        public static TaskScheduler TaskScheduler
        {
            get { return _taskScheduler ?? TaskScheduler.Current; }
        }

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <param name="view">The view to search.</param>
        /// <returns>The root element that was not created by the framework.</returns>
        public static object GetFirstNonGeneratedView(object view)
        {
            return ViewAdapter.GetFirstNonGeneratedView(view);
        }

        /// <summary>
        /// Executes the handler the fist time the view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnFirstLoad(object view, Action<object> handler)
        {
            ViewAdapter.ExecuteOnFirstLoad(view, handler);
        }

        /// <summary>
        /// Executes the handler the next time the view's LayoutUpdated event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnLayoutUpdated(object view, Action<object> handler)
        {
            ViewAdapter.ExecuteOnLayoutUpdated(view, handler);
        }

        /// <summary>
        /// Tries to close the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model to close.</param>
        /// <param name="views">The associated views.</param>
        /// <param name="dialogResult">The dialog result.</param>
        /// <returns>An <see cref="Action"/> to close the view model.</returns>
        public static void TryClose(object viewModel, ICollection<object> views, bool? dialogResult)
        {
            ViewAdapter.TryClose(viewModel, views, dialogResult);
        }
    }
}
