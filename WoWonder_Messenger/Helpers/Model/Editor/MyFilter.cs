﻿using Android.Graphics;
using WoWonder.NiceArt.Models;

namespace PixelPhoto.Helpers.Model.Editor
{
    public class MyFilter
    {
        public int Id { set; get; }
        public Bitmap NameImage { set; get; }
        public PhotoFilter PhotoFilter { set; get; }
    }
}