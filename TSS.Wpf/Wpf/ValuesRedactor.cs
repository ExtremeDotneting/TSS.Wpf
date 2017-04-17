using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using TSS.Helpers;
using TSS.Another;

namespace TSS.Wpf
{
    /// <summary>
    /// The class creates on the transferred StackPanel tools for editing fields objectWithValues object. In my program I edit ConstsUniverse.
    /// <para></para>
    /// Класс создает на передаваемой StackPanel средства для редактирования полей объекта objectWithValues. В моей программе я редактирую ConstsUniverse.
    /// </summary>
    class ValuesRedactor
    {
        /*This inner class is used to store a pointer to the control that is used to edit the field.
        And it stores the minimum and maximum value for the field(if the restriction is not necessary - make Min> Max) 
        Этот внутренний класс используется чтобы хранить указатель на элемент управления, который используется для редактирования поля.
        А также в нем хранится минимальное и максимальное значение для данного поля (если ограничение не нужно - сделайте Min>Max).*/
        class ControlAndInfo
        {
            public Control Control;
            public double Min=2,Max=0;

            public ControlAndInfo(Control control, double min, double max)
            {
                Control = control;
                Min = min;
                Max = max;
            }
            public ControlAndInfo(Control control)
            {
                Control = control;
            }
            public ControlAndInfo()
            {
            }
        }

        /// <summary>
        /// The dictionary, where the key serves the field name, and value - ControlAndInfo, which is edited by the user.
        /// <para></para>
        /// Словарь, где ключем выступает имя поля, а значение - ControlAndInfo, который редактируется пользователем.
        /// </summary>
        Dictionary<string, ControlAndInfo> controlsDictionary = new Dictionary<string, ControlAndInfo>();
        object objectWithValues;
        public object ObjectWithValues
        {
            get { return objectWithValues; }
            private set { objectWithValues=value; }
        }

        public ValuesRedactor(object objectWithValues)
        {
            ObjectWithValues = objectWithValues;
        }

        /// <summary>
        /// Creates editing items on the transferred panel.
        /// <para></para>
        /// Создает элементы редактирования на передаваемой панели. 
        /// </summary>
        public void CreateControlsForObject(StackPanel panel)
        {
            panel.Children.Clear();
            Type objType = ObjectWithValues.GetType();
            controlsDictionary = new Dictionary<string, ControlAndInfo>();
            foreach (FieldInfo fieldInfo in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) )
            {
                if (typeof(IFormattable).IsAssignableFrom(fieldInfo.FieldType) || typeof(bool).IsAssignableFrom(fieldInfo.FieldType))
                {
                    Type fieldType = fieldInfo.FieldType;
                    object fieldValue = fieldInfo.GetValue(ObjectWithValues);
                    GroupBox groupBox = new GroupBox();
                    groupBox.Padding = new Thickness(5, groupBox.FontSize*0.7 + 5, 5, 5);
                    groupBox.Header=fieldInfo.Name;
                    
                    ControlAndInfo controlAndInfo = new ControlAndInfo();
                    Control control;
                    NumericValuesAttribute atr = fieldInfo.GetCustomAttribute<NumericValuesAttribute>();

                    if (typeof(bool).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        CheckBox cb = new CheckBox();
                        cb.IsChecked = Convert.ToBoolean(fieldValue);
                        control = cb;
                    }
                    else if (atr == null)
                    {
                        control = UseTextBox(fieldValue );
                    }
                    else if (atr.WayToShow == NumericValuesWayToShow.Default)
                    {
                        control = UseTextBox(fieldValue);
                        controlAndInfo.Min = atr.Min;
                        controlAndInfo.Max = atr.Max;
                    }
                    else if (atr.WayToShow == NumericValuesWayToShow.Slider)
                    {
                        SliderWithValueLabel swl = new SliderWithValueLabel();
                        swl.slider.Minimum = atr.Min;
                        swl.slider.Maximum = atr.Max;
                        swl.slider.Value = Convert.ToDouble(fieldValue);
                        control = swl;
                    }
                    else
                    {
                        throw new Exception("Don`t know how to show field " + fieldInfo.Name+".");
                    }
                    //////    
                    //content.Margin = new Thickness(5, 0, 5, 0);
                    controlAndInfo.Control = control;
                    controlsDictionary.Add(fieldInfo.Name, controlAndInfo);
                    groupBox.Content = control;
                    panel.Children.Add(groupBox);
                }
            }
        }

        /// <summary>
        /// When you call a function it is an attempt to read the values and write them in the field objectWithValues.If all goes well - will return true.
        /// <para></para>
        /// При вызове функции происходит попытка считать значения и записать их в поля objectWithValues. Если все прошло успешно - вернет истину.
        /// </summary>        
        public bool ConfirmChanges()
        {
            Type objType = ObjectWithValues.GetType();
            Dictionary<string, object> fieldNameAndValueDictionary = new Dictionary<string, object>();
            foreach (var item in controlsDictionary)
            {
                FieldInfo fieldInfo = objType.GetField(item.Key, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                Type fieldType = fieldInfo.FieldType;
                string handledErrorMessage;
                object controlValue = TryToConvert(out handledErrorMessage, item.Value, fieldInfo);
                if (!string.IsNullOrWhiteSpace(handledErrorMessage))
                {
                    MessageBox.Show(handledErrorMessage);
                    item.Value.Control.Focus();
                    return false;
                }
                fieldNameAndValueDictionary.Add(item.Key, controlValue);
            }
            foreach (var item in fieldNameAndValueDictionary)
            {
                FieldInfo fieldInfo = objType.GetField(item.Key, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                fieldInfo.SetValue(
                    ObjectWithValues,
                    item.Value
                    );
            }
            return true;
        }

        /// <summary>
        /// This function try to convert ControlAndInfo values. If the type of error when converting will be ParsebleException, function throws it.
        /// All other errors are processed and return an error message via errorMessage.
        /// <para></para>
        /// Эта функция пытается конвертировать значения ControlAndInfo. Если тип ошибки при конвертировании будет ParsebleException, то она выкинет ее.
        /// Все остальные ошибки обработаются и вернут сообщени об ошибке через errorMessage.
        /// </summary>
        object TryToConvert(out string errorMessage, ControlAndInfo controlAndInfo, FieldInfo fieldInfo)
        {
            //Shit-code, i know. But it`s only way i find to make it work.
            Type fieldType = fieldInfo.FieldType;
            object controlValue = null;
            string errMsg = "";
            try
            {
                controlValue = GetValueFromControl(controlAndInfo.Control, fieldType);
                if (controlValue == null)
                    throw new Exception();
            }
            catch (Exception ex)
            {
                if (typeof(ParsebleException).IsAssignableFrom(ex.GetType()))
                {
                    throw;
                }
                else
                {
                    errMsg += string.Format(LanguageHandler.GetInstance().IncorrectValueMsg, fieldInfo.Name, fieldType.Name);
                }
            }
            double min = controlAndInfo.Min, max = controlAndInfo.Max;
            try
            {
                if (min < max)
                {
                    double value = Convert.ToDouble(GetValueFromControl(controlAndInfo.Control));
                    if (!(value >= min && value <= max))
                        throw new Exception();
                }
            }
            catch
            {
                errMsg += string.Format(LanguageHandler.GetInstance().IncorrectRangeMsg, fieldInfo.Name, min, max);
            }
            errorMessage = errMsg;
            return controlValue;
        }

        /// <summary>
        /// The purpose of this function - read the value from the Control and converted to type.
        /// <para></para>
        /// Задача этой функции - считать значение с Control и преобразовать к типу.
        /// </summary>
        object GetValueFromControl(Control control, Type typeToConvert=null)
        {
            object controlValue=null;
            if (typeof(TextBox).IsAssignableFrom(control.GetType()))
            {
                controlValue = (control as TextBox).Text;
            }
            else if (typeof(SliderWithValueLabel).IsAssignableFrom(control.GetType()))
            {
                controlValue = (control as SliderWithValueLabel).slider.Value;
            }
            else if (typeof(CheckBox).IsAssignableFrom(control.GetType()))
            {
                controlValue = (control as CheckBox).IsChecked;
            }
            else
            {
                throw new Exception("Unknown control!");
            }

            if (typeToConvert == null)
                return controlValue;

            object res;
            if (typeToConvert.GetCustomAttribute<ParsebleAttribute>() != null)
            {
                res = ParsebleAttribute.Parse((string)controlValue, typeToConvert);
            }
            else
            {
                res=Convert.ChangeType(controlValue, typeToConvert);
            }
            return res;
        }
        TextBox UseTextBox(object fieldValue)
        {
            TextBox tb = new TextBox();
            tb.Text = Convert.ToString(fieldValue);
            return tb;
        }  

    }
}
