using System;

namespace dotNetFractal
{
	/// <summary>
	/// Summary description for FractalColor.
	/// </summary>
	public struct FractalColor
	{
		private int _red;
		private int _green;
		private int _blue;
        private int _index;

		public FractalColor(int red, int green, int blue, int index)
		{
			this._red = red;
			this._green = green;
			this._blue = blue;
            this._index = index;
		}

		public override string ToString()
		{
			return (String.Format("({0}, {1}, {2}, {3}", this._red, this._green, this._blue, this._index));
		}

		public int red
		{
			get { return this._red; }
			set { this._red = value; }
		}

		public int green
		{
			get { return this._green; }
			set { this._green = value; }
		}

		public int blue
		{
			get { return this._blue; }
			set { this._blue = value; }
		}

        public int index
        {
            get { return this._index; }
            set { this._index = value; }
        }
    };
}
