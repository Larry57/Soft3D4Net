﻿ namespace Soft3D4Net.WinForms {
    public class PropertyChangedHelper {
        public static bool ChangeValue<T>(ref T oldValue, T newValue) {
            if(Equals(oldValue, newValue))
                return false;

            oldValue = newValue;
            return true;
        }
    }
}
