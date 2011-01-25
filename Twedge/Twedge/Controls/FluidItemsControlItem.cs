﻿using System.Windows;
using System.Windows.Controls;

namespace Twedge.Controls
{
    [TemplateVisualState(Name = "BeforeLoaded", GroupName = "LayoutStates")]
    [TemplateVisualState(Name = "AfterLoaded", GroupName = "LayoutStates")]
    [TemplateVisualState(Name = "BeforeUnloaded", GroupName = "LayoutStates")]
    public class FluidItemsControlItem : ContentControl
    {
    }
}