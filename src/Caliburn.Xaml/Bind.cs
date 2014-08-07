﻿using System.Globalization;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Caliburn.Light
{
    /// <summary>
    /// Hosts attached properties related to View-First.
    /// </summary>
    public static class Bind
    {
        /// <summary>
        /// Allows binding on an existing view. Use this on root UserControls, Pages and Windows; not in a DataTemplate.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached("Model",
            typeof (object), typeof (Bind), new PropertyMetadata(null, OnModelChanged));

        /// <summary>
        /// Allows binding on an existing view without setting the data context. Use this from within a DataTemplate.
        /// </summary>
        public static readonly DependencyProperty ModelWithoutContextProperty =
            DependencyProperty.RegisterAttached("ModelWithoutContext", typeof (object), typeof (Bind),
                new PropertyMetadata(null, OnModelWithoutContextChanged));

        private static readonly DependencyProperty NoDataContextProperty =
            DependencyProperty.RegisterAttached("NoDataContext", typeof (bool), typeof (Bind), null);

        /// <summary>
        /// Gets whether or not the DataContext should be set on the view.
        /// </summary>
        /// <param name="dependencyObject">The view.</param>
        /// <returns>Whether or not the DataContext should be set.</returns>
        public static bool GetNoDataContext(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(NoDataContextProperty);
        }

        /// <summary>
        /// Gets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <returns>The model.</returns>
        public static object GetModelWithoutContext(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelWithoutContextProperty);
        }

        /// <summary>
        /// Sets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <param name = "value">The model.</param>
        public static void SetModelWithoutContext(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelWithoutContextProperty, value);
        }

        /// <summary>
        /// Gets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ModelProperty);
        }

        /// <summary>
        /// Sets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <param name = "value">The model.</param>
        public static void SetModel(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ModelProperty, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ViewHelper.IsInDesignTool || e.NewValue == null || e.NewValue == e.OldValue)
                return;

            var fe = d as FrameworkElement;
            if (fe == null) return;

            SetModelCore(e.NewValue, fe);
        }

        private static void OnModelWithoutContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ViewHelper.IsInDesignTool || e.NewValue == null || e.NewValue == e.OldValue)
                return;

            var fe = d as FrameworkElement;
            if (fe == null) return;

            fe.SetValue(NoDataContextProperty, true);
            SetModelCore(e.NewValue, fe);
        }

        private static void SetModelCore(object viewModel, FrameworkElement view)
        {
            var context = string.IsNullOrEmpty(view.Name)
                ? view.GetHashCode().ToString(CultureInfo.InvariantCulture)
                : view.Name;

            ViewModelBinder.Bind(viewModel, view, context);
        }

        /// <summary>
        /// Allows application of conventions at design-time.
        /// </summary>
        public static readonly DependencyProperty AtDesignTimeProperty =
            DependencyProperty.RegisterAttached("AtDesignTime", typeof (bool), typeof (Bind),
                new PropertyMetadata(false, OnAtDesignTimeChanged));

        private static readonly DependencyProperty DesignDataContextProperty =
            DependencyProperty.RegisterAttached("DesignDataContext", typeof (object), typeof (Bind),
                new PropertyMetadata(null, OnDesignDataContextChanged));

        /// <summary>
        /// Gets whether or not conventions are being applied at design-time.
        /// </summary>
        /// <param name="dependencyObject">The ui to apply conventions to.</param>
        /// <returns>Whether or not conventions are applied.</returns>
        public static bool GetAtDesignTime(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(AtDesignTimeProperty);
        }

        /// <summary>
        /// Sets whether or not do bind conventions at design-time.
        /// </summary>
        /// <param name="dependencyObject">The ui to apply conventions to.</param>
        /// <param name="value">Whether or not to apply conventions.</param>
        public static void SetAtDesignTime(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(AtDesignTimeProperty, value);
        }

        private static void OnAtDesignTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!ViewHelper.IsInDesignTool) return;

            var enabled = (bool) e.NewValue;
            if (!enabled) return;

            BindingHelper.SetBinding(d, DesignDataContextProperty, new Binding());
        }

        private static void OnDesignDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!ViewHelper.IsInDesignTool || e.NewValue == null || e.NewValue == e.OldValue) return;

            var enabled = (bool) d.GetValue(AtDesignTimeProperty);
            if (!enabled) return;

            var fe = d as FrameworkElement;
            if (fe == null) return;

            fe.SetValue(NoDataContextProperty, true);
            SetModelCore(e.NewValue, fe);
        }
    }
}
