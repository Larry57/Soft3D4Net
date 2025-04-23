﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Soft3D4Net.WinForms {

    // Slider

    public partial class Slider : UserControl {
        public event EventHandler ValueChanged {
            add {
                superSlider1.ValueChanged += value;
            }
            remove {
                superSlider1.ValueChanged -= value;
            }
        }

        public Slider() {
            InitializeComponent();
            textBox1.DataBindings.Add(nameof(TextBox.Text), superSlider1, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text {
            get => label1.Text;
            set => label1.Text = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value {
            get => superSlider1.Value;
            set => superSlider1.Value = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Min {
            get => superSlider1.Min;
            set => superSlider1.Min = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Max {
            get => superSlider1.Max;
            set => superSlider1.Max= value;
        }
    }
}
