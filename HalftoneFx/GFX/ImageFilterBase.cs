using System;
using System.Drawing;

namespace HalftoneFx.GFX
{
    public abstract class ImageFilterBase
    {
        private int value;

        private int minValue;

        private int maxValue;
        
        public virtual int Value
        {
            get => this.value;
            set => this.value = this.Clamp(value, this.MinValue, this.MaxValue);
        }

        public int MinValue 
        { 
            get => this.minValue;
            
            set
            {
                if (this.minValue != value)
                {
                    this.minValue = value;

                    if (this.Value < value)
                    {
                        this.Value = value;
                    }
                }
            }
        }

        public int MaxValue
        {
            get => this.maxValue;

            set
            {
                if (this.maxValue != value)
                {
                    this.maxValue = value;

                    if (this.Value > value)
                    {
                        this.Value = value;
                    }
                }
            }
        }

        protected int Clamp(int value, int from, int to)
        {
            return value < from ? from : (value > to ? to : value);
        }

        protected byte ClampByte(int value)
        {
            return (byte)this.Clamp(value, byte.MinValue, byte.MaxValue);
        }

        protected float Snap(float value, float step)
        {
            if (step == 0)
            {
                return value;
            }

            return (float)Math.Floor((value + 0.5F * step) / step) * step;
        }
    }
}
