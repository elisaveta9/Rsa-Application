﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Rsa_Application.Common.Services
{
    internal class DisplayRootRegistry
    {
        readonly Dictionary<Type, Type> vmToWindowMapping = new();

        public void RegisterWindowType<VM, Win>() where Win : Window, new() where VM : class
        {
            var vmType = typeof(VM);
            if (vmType.IsInterface)
                throw new ArgumentException("Cannot register interfaces");
            if (vmToWindowMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Type {vmType.FullName} is already registered");
            vmToWindowMapping[vmType] = typeof(Win);
        }

        public void UnregisterWindowType<VM>()
        {
            var vmType = typeof(VM);
            if (vmType.IsInterface)
                throw new ArgumentException("Cannot register interfaces");
            if (!vmToWindowMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Type {vmType.FullName} is not registered");
            vmToWindowMapping.Remove(vmType);
        }

        public Window CreateWindowInstanceWithVM(object vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));
            Type windowType = null;

            var vmType = vm.GetType();
            while (vmType != null && !vmToWindowMapping.TryGetValue(vmType, out windowType))
                vmType = vmType.BaseType;

            if (windowType == null)
                throw new ArgumentException(
                    $"No registered window type for argument type {vm.GetType().FullName}");

            var window = (Window)Activator.CreateInstance(windowType);
            window.DataContext = vm;
            return window;
        }

        readonly Dictionary<object, Window> openWindows = new();
        public void ShowPresentation(object vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));
            if (openWindows.ContainsKey(vm))
                throw new InvalidOperationException("UI for this VM is already displayed");
            var window = CreateWindowInstanceWithVM(vm);
            window.Show();
            openWindows[vm] = window;
        }

        public void HidePresentation(object vm)
        {
            if (!openWindows.TryGetValue(vm, out Window window))
                throw new InvalidOperationException("UI for this VM is not displayed");
            window.Close();
            openWindows.Remove(vm);
        }

        public async Task ShowModalPresentation(object vm)
        {
            var window = CreateWindowInstanceWithVM(vm);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            await window.Dispatcher.InvokeAsync(() => window.ShowDialog());
        }
    }
}
