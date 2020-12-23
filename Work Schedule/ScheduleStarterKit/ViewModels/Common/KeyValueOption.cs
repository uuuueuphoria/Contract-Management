using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleStarterKit.ViewModels.Common
{
    /// <summary>
    /// This View Model class is ideal for representing data that will utimately be displayed in a DropDownList, RadioButtonList or CheckBoxList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class KeyValueOption<T>
	{
		public T Key { internal get; set; }
		public string DisplayValue => Key.ToString();
		public string DisplayText { get; set; }
	}
}
