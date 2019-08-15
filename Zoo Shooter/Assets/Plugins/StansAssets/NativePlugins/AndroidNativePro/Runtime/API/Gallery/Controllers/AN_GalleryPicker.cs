using System;
using System.Collections.Generic;
using SA.Android.App;
using SA.Android.Gallery.Internal;
using SA.Android.Manifest;

namespace SA.Android.Gallery
{
    /// <summary>
    /// Picker object for picking media from the device storage
    /// </summary>
    public class AN_MediaPicker 
    {

        private int m_maxSize = 512;
        private bool m_allowMultiSelect;

        private List<AN_MediaType> m_types;

        /// <summary>
        /// Create new instance of the picker with predefined picker types
        /// </summary>
        /// <param name="types"></param>
        public AN_MediaPicker(params AN_MediaType[] types) {
            m_types = new List<AN_MediaType>(types);
        }


        /// <summary>
        /// Max thumbnail size that will be transferred to the Unity side.
        /// The thumbnail will be resized before it sent.
        /// The default value is 512.
        /// </summary>
        public int MaxSize {
            get {
                return m_maxSize;
            }

            set {
                m_maxSize = value;
            }
        }

        /// <summary>
        /// Defines if multiple images picker is allowed.
        /// The default value is <c>false</c>
        /// </summary>
        public bool AllowMultiSelect {
            get {
                return m_allowMultiSelect;
            }

            set {
                m_allowMultiSelect = value;
            }
        }

        /// <summary>
        /// Starts pick media from a gallery flow.
        /// </summary>
        /// <param name="callback"></param>
        public void Show(Action<AN_GalleryPickResult> callback) {
            
            AN_PermissionsUtility.TryToResolvePermission(
                new [] { AMM_ManifestPermission.READ_EXTERNAL_STORAGE, AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE },
                (granted) => {
                    var type = AN_GalleryChooseType.PICK_PICTURE;
                    if(m_types.Contains(AN_MediaType.Image) && m_types.Contains(AN_MediaType.Video)) {
                        type = AN_GalleryChooseType.PICK_PICTURE_OR_VIDEO;
                    } else {
                        if(m_types.Contains(AN_MediaType.Video)) {
                            type = AN_GalleryChooseType.PICK_VIDEO;
                        }
                    }

                    AN_GalleryInternal.PickImageFromGallery(m_maxSize, type, m_allowMultiSelect, callback);
                });
            

           
        }


    }
}