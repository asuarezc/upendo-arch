﻿using System;
using System.Threading.Tasks;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface ICameraAndGalleryPageService
    {
        Task<MediaItem> CapturePhotoAsync();
        Task<MediaItem> CaptureVideoAsync();
        Task<MediaItem> PickPhotoAsync();
        Task<MediaItem> PickVideoAsync();
    }
}