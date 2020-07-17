﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.Media;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Request.Transition;
using Com.Luseen.Autolinklibrary;
using Java.IO;
using WoWonder.Activities.DefaultUser;
using WoWonder.Adapters;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.Library.MusicBar;
using WoWonderClient.Classes.Message;
using Console = System.Console;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;
using MessageData = WoWonder.Helpers.Model.MessageDataExtra;
using Priority = Bumptech.Glide.Priority;
using System.Collections.Generic;

namespace WoWonder.Activities.GroupChat.Adapter
{
    public class GroupMessagesAdapter : RecyclerView.Adapter, MusicBar.IOnMusicBarAnimationChangeListener, MusicBar.IOnMusicBarProgressChangeListener, ValueAnimator.IAnimatorUpdateListener
    {
        public event EventHandler<Holders.MesClickEventArgs> ItemClick;
        public event EventHandler<Holders.MesClickEventArgs> ItemLongClick;

        public ObservableCollection<AdapterModelsClassGroup> DifferList = new ObservableCollection<AdapterModelsClassGroup>();
        private readonly Activity MainActivity;
        private readonly RequestOptions Options;
        private readonly RequestBuilder FullGlideRequestBuilder;
        private readonly string GroupId;

        public GroupMessagesAdapter(Activity activity, string groupId)
        {
            try
            {
               //HasStableIds = true;
                MainActivity = activity;
                GroupId = groupId;
                DifferList = new ObservableCollection<AdapterModelsClassGroup>();

                Options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                    .CenterCrop()
                    .SetPriority(Priority.High).Override(200)
                    .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                    .Error(Resource.Drawable.ImagePlacholder)
                    .Placeholder(Resource.Drawable.ImagePlacholder));
                FullGlideRequestBuilder = Glide.With(MainActivity).AsDrawable().Apply(new RequestOptions().Apply(RequestOptions.CircleCropTransform().CenterCrop().Transform(new MultiTransformation(new CenterCrop(), new RoundedCorners(25))).SetPriority(Priority.High).Override(450).SetDiskCacheStrategy(DiskCacheStrategy.Automatic).SkipMemoryCache(true).SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All).Placeholder(new ColorDrawable(Color.ParseColor("#888888")))));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                switch (viewType)
                {
                    case (int)MessageModelType.RightProduct:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Products, parent, false);
                            Holders.ProductViewHolder viewHolder = new Holders.ProductViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.LeftProduct:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Products, parent, false);
                            Holders.ProductViewHolder viewHolder = new Holders.ProductViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.RightText:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_view, parent, false);
                            Holders.TextViewHolder textViewHolder = new Holders.TextViewHolder(row, OnClick, OnLongClick, false);
                            return textViewHolder;
                        }
                    case (int)MessageModelType.LeftText:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                            Holders.TextViewHolder textViewHolder = new Holders.TextViewHolder(row, OnClick, OnLongClick, false);
                            return textViewHolder;
                        }
                    case (int)MessageModelType.RightImage:
                    case (int)MessageModelType.RightGif:
                    case (int)MessageModelType.RightMap:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_image, parent, false);
                            Holders.ImageViewHolder imageViewHolder = new Holders.ImageViewHolder(row, OnClick, OnLongClick, false);
                            return imageViewHolder;
                        }
                    case (int)MessageModelType.LeftImage:
                    case (int)MessageModelType.LeftGif:
                    case (int)MessageModelType.LeftMap:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_image, parent, false);
                            Holders.ImageViewHolder imageViewHolder = new Holders.ImageViewHolder(row, OnClick, OnLongClick, false);
                            return imageViewHolder;
                        }
                    case (int)MessageModelType.RightAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_AudioBar, parent, false);
                                Holders.MusicBarViewHolder soundViewHolder = new Holders.MusicBarViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                            else
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Audio, parent, false);
                                Holders.SoundViewHolder soundViewHolder = new Holders.SoundViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                        }
                    case (int)MessageModelType.LeftAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_AudioBar, parent, false);
                                Holders.MusicBarViewHolder soundViewHolder = new Holders.MusicBarViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                            else
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Audio, parent, false);
                                Holders.SoundViewHolder soundViewHolder = new Holders.SoundViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                        }
                    case (int)MessageModelType.RightContact:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Contact, parent, false);
                            Holders.ContactViewHolder contactViewHolder = new Holders.ContactViewHolder(row, OnClick, OnLongClick, false);
                            return contactViewHolder;
                        }
                    case (int)MessageModelType.LeftContact:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Contact, parent, false);
                            Holders.ContactViewHolder contactViewHolder = new Holders.ContactViewHolder(row, OnClick, OnLongClick, false);
                            return contactViewHolder;
                        }
                    case (int)MessageModelType.RightVideo:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Video, parent, false);
                            Holders.VideoViewHolder videoViewHolder = new Holders.VideoViewHolder(row, OnClick, OnLongClick, false);
                            return videoViewHolder;
                        }
                    case (int)MessageModelType.LeftVideo:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Video, parent, false);
                            Holders.VideoViewHolder videoViewHolder = new Holders.VideoViewHolder(row, OnClick, OnLongClick, false);
                            return videoViewHolder;
                        }
                    case (int)MessageModelType.RightSticker:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_sticker, parent, false);
                            Holders.StickerViewHolder stickerViewHolder = new Holders.StickerViewHolder(row, OnClick, OnLongClick, false);
                            return stickerViewHolder;
                        }
                    case (int)MessageModelType.LeftSticker:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_sticker, parent, false);
                            Holders.StickerViewHolder stickerViewHolder = new Holders.StickerViewHolder(row, OnClick, OnLongClick, false);
                            return stickerViewHolder;
                        }
                    case (int)MessageModelType.RightFile:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_file, parent, false);
                            Holders.FileViewHolder viewHolder = new Holders.FileViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.LeftFile:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_file, parent, false);
                            Holders.FileViewHolder viewHolder = new Holders.FileViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    default:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                            Holders.NotSupportedViewHolder viewHolder = new Holders.NotSupportedViewHolder(row);
                            return viewHolder;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder vh, int position)
        {
            try
            {
                var item = DifferList[position];
                if (item == null)
                    return;

                var itemViewType = vh.ItemViewType;
                switch (itemViewType)
                {
                    case (int)MessageModelType.RightProduct:
                    case (int)MessageModelType.LeftProduct:
                        {
                            Holders.ProductViewHolder holder = vh as Holders.ProductViewHolder;
                            LoadProductOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightGif:
                    case (int)MessageModelType.LeftGif:
                        {
                            Holders.ImageViewHolder holder = vh as Holders.ImageViewHolder;
                            LoadGifOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightText:
                    case (int)MessageModelType.LeftText:
                        {
                            Holders.TextViewHolder holder = vh as Holders.TextViewHolder;
                            LoadTextOfchatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightImage:
                    case (int)MessageModelType.LeftImage:
                        {
                            Holders.ImageViewHolder holder = vh as Holders.ImageViewHolder;
                            LoadImageOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightMap:
                    case (int)MessageModelType.LeftMap:
                        {
                            Holders.ImageViewHolder holder = vh as Holders.ImageViewHolder;
                            LoadMapOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightAudio:
                    case (int)MessageModelType.LeftAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                Holders.MusicBarViewHolder holder = vh as Holders.MusicBarViewHolder;
                                LoadAudioBarOfChatItem(holder, position, item.MesData);
                                break;
                            }
                            else
                            {
                                Holders.SoundViewHolder holder = vh as Holders.SoundViewHolder;
                                LoadAudioOfChatItem(holder, position, item.MesData);
                                break;
                            }
                        }
                    case (int)MessageModelType.RightContact:
                    case (int)MessageModelType.LeftContact:
                        {
                            Holders.ContactViewHolder holder = vh as Holders.ContactViewHolder;
                            LoadContactOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightVideo:
                    case (int)MessageModelType.LeftVideo:
                        {
                            Holders.VideoViewHolder holder = vh as Holders.VideoViewHolder;
                            LoadVideoOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightSticker:
                    case (int)MessageModelType.LeftSticker:
                        {
                            Holders.StickerViewHolder holder = vh as Holders.StickerViewHolder;
                            LoadStickerOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightFile:
                    case (int)MessageModelType.LeftFile:
                        {
                            Holders.FileViewHolder holder = vh as Holders.FileViewHolder;
                            LoadFileOfChatItem(holder, position, item.MesData);
                            break;
                        }
                    default:
                        {
                            if (!string.IsNullOrEmpty(item.MesData.Text))
                            {
                                if (vh is Holders.TextViewHolder holderText)
                                {
                                    LoadTextOfchatItem(holderText, position, item.MesData);
                                }
                            }
                            else
                            {
                                if (vh is Holders.NotSupportedViewHolder holder)
                                    holder.AutoLinkNotsupportedView.Text = MainActivity.GetText(Resource.String.Lbl_TextChatNotSupported);
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position, IList<Object> payloads)
        {
            try
            {
                if (payloads.Count > 0)
                {
                    var item = DifferList[position];

                    switch (payloads[0].ToString())
                    {
                        case "WithoutBlobImage":
                        case "WithoutBlobGIF":
                            {
                                if (viewHolder is Holders.ImageViewHolder holder)
                                    LoadImageOfChatItem(holder, position, item.MesData);
                                //NotifyItemChanged(position);
                                break;
                            }
                        case "WithoutBlobVideo":
                            {
                                if (viewHolder is Holders.VideoViewHolder holder)
                                    LoadVideoOfChatItem(holder, position, item.MesData);
                                //NotifyItemChanged(position);
                                break;
                            }
                        case "WithoutBlobAudio":
                            {
                                if (AppSettings.ShowMusicBar)
                                {
                                    if (viewHolder is Holders.MusicBarViewHolder holder)
                                        LoadAudioBarOfChatItem(holder, position, item.MesData);
                                    //NotifyItemChanged(position);
                                    break;
                                }
                                else
                                {
                                    if (viewHolder is Holders.SoundViewHolder holder)
                                        LoadAudioOfChatItem(holder, position, item.MesData);
                                    //NotifyItemChanged(position);
                                    break;
                                }
                            }
                        case "WithoutBlobFile":
                            {
                                if (viewHolder is Holders.FileViewHolder holder)
                                    LoadFileOfChatItem(holder, position, item.MesData);
                                //NotifyItemChanged(position);
                                break;
                            }
                        default:
                            base.OnBindViewHolder(viewHolder, position, payloads);
                            break;
                    }
                }
                else
                {
                    base.OnBindViewHolder(viewHolder, position, payloads);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnBindViewHolder(viewHolder, position, payloads);
            }
        }

        //==============================

        #region Function Load Message

        //private void SetStartedMessage(LottieAnimationView favAnimationView, ImageView starImage, bool star)
        //{
        //    try
        //    {
        //        if (favAnimationView != null)
        //        {
        //            if (star)
        //            {
        //                favAnimationView.PlayAnimation();
        //                favAnimationView.Visibility = ViewStates.Visible;
        //                starImage.SetImageResource(Resource.Drawable.icon_star_filled_post_vector);
        //                starImage.Visibility = ViewStates.Visible;
        //            }
        //            else
        //            {
        //                favAnimationView.Progress = 0;
        //                favAnimationView.CancelAnimation();
        //                favAnimationView.Visibility = ViewStates.Gone;
        //                starImage.SetImageResource(Resource.Drawable.icon_fav_post_vector);
        //                starImage.Visibility = ViewStates.Gone;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

        private void SetSeenMessage(TextView view, string seen)
        {
            try
            {
                if (view != null)
                {
                    if (seen == "0")
                    {
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, view, FontAwesomeIcon.Check);
                        view.SetTextColor(Color.ParseColor("#efefef"));
                    }
                    else
                    {
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, view, FontAwesomeIcon.CheckDouble);
                        view.SetTextColor(Color.Blue);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadTextOfchatItem(Holders.TextViewHolder holder, int position, MessageData message)
        {
            try
            {
                Console.WriteLine(position);

                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                holder.Time.Text = message.TimeText;

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                holder.Time.Visibility = message.ShowTimeText ? ViewStates.Visible : ViewStates.Gone;

                if (message.ModelType == MessageModelType.LeftText)
                {
                    holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                    holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModePhone_color));
                    holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeEmail_color));
                    holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeHashtag_color));
                    holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeUrl_color));
                    holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeMention_color));
                }
                else
                {
                    holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                    holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModePhone_color));
                    holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeEmail_color));
                    holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeHashtag_color));
                    holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeUrl_color));
                    holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeMention_color));
                }
                holder.AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;

                holder.AutoLinkTextView.SetAutoLinkText(message.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AutoLinkTextViewOnAutoLinkOnClick(object sender1, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email")
                {
                    Methods.App.SendEmail(MainActivity, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website")
                {
                    var url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }

                    Methods.App.OpenWebsiteUrl(MainActivity, url);
                }
                else if (typetext == "Hashtag")
                {

                }
                else if (typetext == "Mention")
                {
                    var intent = new Intent(MainActivity, typeof(SearchActivity));
                    intent.PutExtra("Key", autoLinkOnClickEventArgs.P1.Replace("@", ""));
                    MainActivity.StartActivity(intent);
                }
                else if (typetext == "Number")
                {
                    Methods.App.SaveContacts(MainActivity, autoLinkOnClickEventArgs.P1, "", "2");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void LoadMapOfChatItem(Holders.ImageViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                Console.WriteLine(position);

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                holder.Time.Text = message.TimeText;

                LatLng latLng = new LatLng(Convert.ToDouble(message.Lat), Convert.ToDouble(message.Lng));

                var addresses = await ReverseGeocodeCurrentLocation(latLng);
                if (addresses != null)
                {
                    var deviceAddress = addresses.GetAddressLine(0);

                    string imageUrlMap = "https://maps.googleapis.com/maps/api/staticmap?";
                    //imageUrlMap += "center=" + item.CurrentLatitude + "," + item.CurrentLongitude;
                    imageUrlMap += "center=" + deviceAddress;
                    imageUrlMap += "&zoom=13";
                    imageUrlMap += "&scale=2";
                    imageUrlMap += "&size=150x150";
                    imageUrlMap += "&maptype=roadmap";
                    imageUrlMap += "&key=" + MainActivity.GetText(Resource.String.google_maps_key);
                    imageUrlMap += "&format=png";
                    imageUrlMap += "&visual_refresh=true";
                    imageUrlMap += "&markers=size:small|color:0xff0000|label:1|" + deviceAddress;

                    FullGlideRequestBuilder.Load(imageUrlMap).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

                if (!holder.ImageView.HasOnClickListeners)
                {
                    holder.ImageView.Click += (sender, args) =>
                    {
                        try
                        {
                            // Create a Uri from an intent string. Use the result to create an Intent. 
                            var uri = Uri.Parse("geo:" + message.Lat + "," + message.Lng);
                            var intent = new Intent(Intent.ActionView, uri);
                            intent.SetPackage("com.google.android.apps.maps");
                            intent.AddFlags(ActivityFlags.NewTask);
                            MainActivity.StartActivity(intent);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Location >> BindMap

        private async Task<Address> ReverseGeocodeCurrentLocation(LatLng latLng)
        {
            try
            {
                var locale = MainActivity.Resources.Configuration.Locale;
                Geocoder geocode = new Geocoder(MainActivity, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                if (addresses.Count > 0)
                {
                    //string address = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                    //string city = addresses[0].Locality;
                    //string state = addresses[0].AdminArea;
                    //string country = addresses[0].CountryName;
                    //string postalCode = addresses[0].PostalCode;
                    //string knownName = addresses[0].FeatureName; // Only if available else return NULL 
                    return addresses.FirstOrDefault();
                }
                else
                {
                    //Error Message  
                    //Toast.MakeText(MainActivity, MainActivity.GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short).Show();
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        private void LoadImageOfChatItem(Holders.ImageViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                var fileName = message.Media.Split('/').Last();
                message.Media = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimImage, fileName, message.Media);

                holder.Time.Text = message.TimeText;

                if (message.Media.Contains("http"))
                {
                    GlideImageLoader.LoadImage(MainActivity, message.Media, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    var file = Uri.FromFile(new File(message.Media));
                    // GlideImageLoader.LoadImage(MainActivity, file.ToString(), holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                    Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadProductOfChatItem(Holders.ProductViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                string imageUrl = !string.IsNullOrEmpty(message.Media) ? message.Media : message.Product?.ProductClass?.Images[0]?.Image;
                holder.Time.Text = message.TimeText;

                holder.Title.Text = message.Product?.ProductClass?.Name;
                holder.Cat.Text = ListUtils.ListCategoriesProducts.FirstOrDefault(a => a.CategoriesId == message.Product?.ProductClass?.Category)?.CategoriesName;

                var (currency, currencyIcon) = WoWonderTools.GetCurrency(message.Product?.ProductClass?.Currency);
                holder.Price.Text = currencyIcon + " " + message.Product?.ProductClass?.Price;
                Console.WriteLine(currency);

                if (imageUrl != null && (imageUrl.Contains("http://") || imageUrl.Contains("https://")))
                {
                    GlideImageLoader.LoadImage(MainActivity, imageUrl, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    var file = Uri.FromFile(new File(imageUrl));
                    Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool MSeekBarIsTracking;
        private ValueAnimator MValueAnimator;
        private MessageData MusicBarViewHolder;
        private int PositionSound;
        private void LoadAudioBarOfChatItem(Holders.MusicBarViewHolder musicBarViewHolder, int position, MessageData message)
        {
            try
            {
                if (musicBarViewHolder.UserName != null)
                {
                    musicBarViewHolder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    musicBarViewHolder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(musicBarViewHolder.Seen, message.Seen);
                    //musicBarViewHolder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(musicBarViewHolder.StarIcon, musicBarViewHolder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                MusicBarViewHolder = message;
                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                message.MusicBarViewHolder = musicBarViewHolder;

                if (message.SendFile)
                {
                    musicBarViewHolder.LoadingProgressview.Indeterminate = true;
                    musicBarViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    musicBarViewHolder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    musicBarViewHolder.LoadingProgressview.Indeterminate = false;
                    musicBarViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    musicBarViewHolder.PlayButton.Visibility = ViewStates.Visible;
                }

                musicBarViewHolder.MsgTimeTextView.Text = message.TimeText;

                var fileName = message.Media.Split('/').Last();

                var mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimSound, fileName, message.Media);

                var durationOfSound = Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile);
                if (string.IsNullOrEmpty(message.MediaDuration))
                    musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(durationOfSound);
                else
                    musicBarViewHolder.DurationTextView.Text = message.MediaDuration;

                if (mediaFile.Contains("http"))
                    mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimSound, fileName, message.Media);

                musicBarViewHolder.FixedMusicBar.LoadFrom(mediaFile, (int)durationOfSound);
                musicBarViewHolder.FixedMusicBar.Show();

                if (!musicBarViewHolder.PlayButton.HasOnClickListeners)
                {
                    musicBarViewHolder.PlayButton.Click += (o, args) =>
                    {
                        try
                        {
                            if (PositionSound != position)
                            {
                                var list = DifferList.Where(a => a.TypeView == MessageModelType.LeftAudio || a.TypeView == MessageModelType.RightAudio && a.MesData.MediaPlayer != null).ToList();
                                if (list.Count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        if (item.MesData.MediaPlayer != null)
                                        {
                                            item.MesData.MediaPlayer.Stop();
                                            item.MesData.MediaPlayer.Reset();
                                        }
                                        item.MesData.MediaPlayer = null;
                                        item.MesData.MediaTimer = null;

                                        item.MesData.MediaPlayer?.Release();
                                        item.MesData.MediaPlayer = null;
                                    }
                                }
                            }

                            if (message.MediaPlayer == null)
                            {
                                PositionSound = position;
                                message.MediaPlayer = new MediaPlayer();
                                message.MediaPlayer.SetAudioAttributes(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build());

                                message.MediaPlayer.Completion += (sender, e) =>
                                {
                                    try
                                    {
                                        musicBarViewHolder.PlayButton.Tag = "Play";
                                        musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                        message.MediaIsPlaying = false;

                                        message.MediaPlayer.Stop();
                                        message.MediaPlayer.Reset();
                                        message.MediaPlayer = null;

                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                        message.MediaTimer = null;

                                        musicBarViewHolder.FixedMusicBar.StopAutoProgress();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };

                                message.MediaPlayer.Prepared += (s, ee) =>
                                {
                                    try
                                    {
                                        message.MediaIsPlaying = true;
                                        musicBarViewHolder.PlayButton.Tag = "Pause";
                                        musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_media_pause_dark : Resource.Drawable.ic_media_pause_light);

                                        if (message.MediaTimer == null)
                                            message.MediaTimer = new Timer { Interval = 1000 };

                                        message.MediaPlayer.Start();

                                        var mediaPlayerDuration = message.MediaPlayer.Duration;

                                        //change bar width
                                        musicBarViewHolder.FixedMusicBar.SetBarWidth(2);

                                        //change Space Between Bars
                                        musicBarViewHolder.FixedMusicBar.SetSpaceBetweenBar(2);

                                        if (mediaFile.Contains("http"))
                                            mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimSound, fileName, message.Media);

                                        musicBarViewHolder.FixedMusicBar.LoadFrom(mediaFile, mediaPlayerDuration);

                                        musicBarViewHolder.FixedMusicBar.SetAnimationChangeListener(this);
                                        musicBarViewHolder.FixedMusicBar.SetProgressChangeListener(this);
                                        InitValueAnimator(1.0f, 0, mediaPlayerDuration);
                                        musicBarViewHolder.FixedMusicBar.Show();

                                        message.MediaTimer.Elapsed += (sender, eventArgs) =>
                                        {
                                            MainActivity.RunOnUiThread(() =>
                                            {
                                                try
                                                {
                                                    if (message.MediaTimer.Enabled)
                                                    {
                                                        if (message.MediaPlayer.CurrentPosition <= message.MediaPlayer.Duration)
                                                        {
                                                            musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.CurrentPosition);
                                                        }
                                                        else
                                                        {
                                                            musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.Duration);

                                                            musicBarViewHolder.PlayButton.Tag = "Play";
                                                            musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    musicBarViewHolder.PlayButton.Tag = "Play";
                                                }
                                            });
                                        };
                                        message.MediaTimer.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                };

                                if (mediaFile.Contains("http"))
                                {
                                    message.MediaPlayer.SetDataSource(MainActivity, Uri.Parse(mediaFile));
                                    message.MediaPlayer.PrepareAsync();
                                }
                                else
                                {
                                    File file2 = new File(mediaFile);
                                    var photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                                    message.MediaPlayer.SetDataSource(MainActivity, photoUri);
                                    message.MediaPlayer.Prepare();
                                }

                                MusicBarViewHolder = message;
                                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                                message.MusicBarViewHolder = musicBarViewHolder;
                            }
                            else
                            {
                                if (musicBarViewHolder.PlayButton.Tag.ToString() == "Play")
                                {
                                    musicBarViewHolder.PlayButton.Tag = "Pause";
                                    musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_media_pause_dark : Resource.Drawable.ic_media_pause_light);

                                    message.MediaIsPlaying = true;
                                    message.MediaPlayer?.Start();

                                    musicBarViewHolder.FixedMusicBar.Show();

                                    InitValueAnimator(1.0f, musicBarViewHolder.FixedMusicBar.GetPosition(), message.MediaPlayer.Duration);

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = true;
                                        message.MediaTimer.Start();
                                    }
                                }
                                else if (musicBarViewHolder.PlayButton.Tag.ToString() == "Pause")
                                {
                                    musicBarViewHolder.PlayButton.Tag = "Play";
                                    musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                    message.MediaIsPlaying = false;
                                    message.MediaPlayer?.Pause();

                                    //stop auto progress animation
                                    musicBarViewHolder.FixedMusicBar.StopAutoProgress();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }

                MusicBarViewHolder = message;
                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                message.MusicBarViewHolder = musicBarViewHolder;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadAudioOfChatItem(Holders.SoundViewHolder soundViewHolder, int position, MessageData message)
        {
            try
            {
                if (soundViewHolder.UserName != null)
                {
                    soundViewHolder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    soundViewHolder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(soundViewHolder.Seen, message.Seen);
                    //soundViewholder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(soundViewHolder.StarIcon, soundViewHolder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                if (message.SoundViewHolder == null)
                    message.SoundViewHolder = soundViewHolder;

                if (message.SendFile)
                {
                    soundViewHolder.LoadingProgressview.Indeterminate = true;
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    soundViewHolder.LoadingProgressview.Indeterminate = false;
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Visible;
                }

                soundViewHolder.MsgTimeTextView.Text = message.TimeText;

                var fileName = message.Media.Split('/').Last();

                var mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimSound, fileName, message.Media);
                soundViewHolder.DurationTextView.Text = string.IsNullOrEmpty(message.MediaDuration)
                    ? Methods.AudioRecorderAndPlayer.GetTimeString(Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile))
                    : message.MediaDuration;

                if (!soundViewHolder.PlayButton.HasOnClickListeners)
                {
                    soundViewHolder.PlayButton.Click += (o, args) =>
                    {
                        try
                        {
                            if (PositionSound != position)
                            {
                                var list = DifferList.Where(a => a.TypeView == MessageModelType.LeftAudio || a.TypeView == MessageModelType.RightAudio && a.MesData.MediaPlayer != null).ToList();
                                if (list.Count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        if (item.MesData.MediaPlayer != null)
                                        {
                                            item.MesData.MediaPlayer.Stop();
                                            item.MesData.MediaPlayer.Reset();
                                        }
                                        item.MesData.MediaPlayer = null;
                                        item.MesData.MediaTimer = null;

                                        item.MesData.MediaPlayer?.Release();
                                        item.MesData.MediaPlayer = null;
                                    }
                                }
                            }

                            if (mediaFile.Contains("http"))
                                mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimSound, fileName, message.Media);

                            if (message.MediaPlayer == null)
                            {
                                PositionSound = position;
                                message.MediaPlayer = new MediaPlayer();
                                message.MediaPlayer.SetAudioAttributes(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build());

                                message.MediaPlayer.Completion += (sender, e) =>
                                {
                                    try
                                    {
                                        soundViewHolder.PlayButton.Tag = "Play";
                                        soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                        message.MediaIsPlaying = false;

                                        message.MediaPlayer.Stop();
                                        message.MediaPlayer.Reset();
                                        message.MediaPlayer = null;

                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                        message.MediaTimer = null;
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };

                                message.MediaPlayer.Prepared += (s, ee) =>
                                {
                                    try
                                    {
                                        message.MediaIsPlaying = true;
                                        soundViewHolder.PlayButton.Tag = "Pause";
                                        if (message.ModelType == MessageModelType.LeftAudio)
                                            soundViewHolder.PlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);
                                        else
                                            soundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_media_pause_light);

                                        if (message.MediaTimer == null)
                                            message.MediaTimer = new Timer { Interval = 1000 };

                                        message.MediaPlayer.Start();

                                        //var durationOfSound = message.MediaPlayer.Duration;

                                        message.MediaTimer.Elapsed += (sender, eventArgs) =>
                                        {
                                            MainActivity.RunOnUiThread(() =>
                                            {
                                                try
                                                {
                                                    if (message.MediaTimer.Enabled)
                                                    {
                                                        if (message.MediaPlayer.CurrentPosition <= message.MediaPlayer.Duration)
                                                        {
                                                            soundViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.CurrentPosition);
                                                        }
                                                        else
                                                        {
                                                            soundViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.Duration);

                                                            soundViewHolder.PlayButton.Tag = "Play";
                                                            soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    soundViewHolder.PlayButton.Tag = "Play";
                                                }
                                            });
                                        };
                                        message.MediaTimer.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                };

                                if (mediaFile.Contains("http"))
                                {
                                    message.MediaPlayer.SetDataSource(MainActivity, Uri.Parse(mediaFile));
                                    message.MediaPlayer.PrepareAsync();
                                }
                                else
                                {
                                    File file2 = new File(mediaFile);
                                    var photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                                    message.MediaPlayer.SetDataSource(MainActivity, photoUri);
                                    message.MediaPlayer.Prepare();
                                }

                                message.SoundViewHolder = soundViewHolder;
                            }
                            else
                            {
                                if (soundViewHolder.PlayButton.Tag.ToString() == "Play")
                                {
                                    soundViewHolder.PlayButton.Tag = "Pause";
                                    if (message.ModelType == MessageModelType.LeftAudio)
                                        soundViewHolder.PlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);
                                    else
                                        soundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_media_pause_light);

                                    message.MediaIsPlaying = true;
                                    message.MediaPlayer?.Start();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = true;
                                        message.MediaTimer.Start();
                                    }
                                }
                                else if (soundViewHolder.PlayButton.Tag.ToString() == "Pause")
                                {
                                    soundViewHolder.PlayButton.Tag = "Play";
                                    soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                    message.MediaIsPlaying = false;
                                    message.MediaPlayer?.Pause();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }

                if (message.SoundViewHolder == null)
                    message.SoundViewHolder = soundViewHolder;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadContactOfChatItem(Holders.ContactViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                holder.MsgTimeTextView.Text = message.TimeText;
                holder.MsgTimeTextView.Visibility = message.ShowTimeText ? ViewStates.Visible : ViewStates.Gone;

                if (!string.IsNullOrEmpty(message.ContactName))
                {
                    holder.UserContactNameTextView.Text = message.ContactName;
                    holder.UserNumberTextView.Text = message.ContactNumber;
                }

                if (!holder.ContactLayout.HasOnClickListeners)
                {
                    holder.ContactLayout.Click += (sender, args) =>
                    {
                        try
                        {
                            Methods.App.SaveContacts(MainActivity, message.ContactNumber, message.ContactName, "2");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadVideoOfChatItem(Holders.VideoViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                var fileName = message.Media.Split('/').Last();
                var fileNameWithoutExtension = fileName.Split('.').First();
                var mediaFile = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimVideo, fileName, message.Media);

                holder.MsgTimeTextView.Text = message.TimeText;
                holder.FilenameTextView.Text = Methods.FunString.SubStringCutOf(fileNameWithoutExtension, 12) + ".mp4";

                var videoImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + GroupId, fileNameWithoutExtension + ".png");
                if (videoImage == "File Dont Exists")
                {
                    File file2 = new File(mediaFile);
                    try
                    {
                        Uri photoUri = message.Media.Contains("http") ? Uri.Parse(message.Media) : FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);
                        Glide.With(MainActivity)
                            .AsBitmap()
                            .Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable))
                            .Load(photoUri) // or URI/path
                            .Into(new MySimpleTarget(this, holder, position));  //image view to set thumbnail to 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Glide.With(MainActivity)
                            .AsBitmap()
                            .Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable))
                            .Load(file2) // or URI/path
                            .Into(new MySimpleTarget(this, holder, position));  //image view to set thumbnail to 
                    }
                }
                else
                {
                    File file = new File(videoImage);
                    try
                    {
                        Uri photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file);
                        Glide.With(MainActivity).Load(photoUri).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Glide.With(MainActivity).Load(file).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                    }
                }

                if (message.SendFile)
                {
                    holder.LoadingProgressview.Indeterminate = true;
                    holder.LoadingProgressview.Visibility = ViewStates.Visible;
                    holder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    holder.LoadingProgressview.Indeterminate = false;
                    holder.LoadingProgressview.Visibility = ViewStates.Gone;
                    holder.PlayButton.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class MySimpleTarget : CustomTarget
        {
            private readonly GroupMessagesAdapter MAdapter;
            private readonly Holders.VideoViewHolder ViewHolder;
            private readonly int Position;
            public MySimpleTarget(GroupMessagesAdapter adapter, Holders.VideoViewHolder viewHolder, int position)
            {
                try
                {
                    MAdapter = adapter;
                    ViewHolder = viewHolder;
                    Position = position;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnResourceReady(Object resource, ITransition transition)
            {
                try
                {
                    if (MAdapter.DifferList?.Count > 0)
                    {
                        var item = MAdapter.DifferList[Position].MesData;
                        if (item != null)
                        {
                            var fileName = item.Media.Split('/').Last();
                            var fileNameWithoutExtension = fileName.Split('.').First();

                            var pathImage = Methods.Path.FolderDcimVideo + MAdapter.GroupId + "/" + fileNameWithoutExtension + ".png";

                            var videoImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + MAdapter.GroupId, fileNameWithoutExtension + ".png");
                            if (videoImage == "File Dont Exists")
                            {
                                if (resource is Bitmap bitmap)
                                {
                                    Methods.MultiMedia.Export_Bitmap_As_Image(bitmap, fileNameWithoutExtension, Methods.Path.FolderDcimVideo + MAdapter.GroupId + "/");

                                    File file2 = new File(pathImage);
                                    var photoUri = FileProvider.GetUriForFile(MAdapter.MainActivity, MAdapter.MainActivity.PackageName + ".fileprovider", file2);

                                    Glide.With(MAdapter.MainActivity).Load(photoUri).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(ViewHolder.ImageView);

                                    item.ImageVideo = photoUri.ToString();
                                }
                            }
                            else
                            {
                                File file2 = new File(pathImage);
                                var photoUri = FileProvider.GetUriForFile(MAdapter.MainActivity, MAdapter.MainActivity.PackageName + ".fileprovider", file2);

                                Glide.With(MAdapter.MainActivity).Load(photoUri).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(ViewHolder.ImageView);

                                item.ImageVideo = photoUri.ToString();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnLoadCleared(Drawable p0) { }
        }

        private void LoadStickerOfChatItem(Holders.StickerViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                    SetSeenMessage(holder.Seen, message.Seen);

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                var fileName = message.Media.Split('/').Last();
                message.Media = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDiskSticker, fileName, message.Media);

                holder.Time.Text = message.TimeText;
                if (message.Media.Contains("http"))
                {
                    Glide.With(MainActivity).Load(message.Media).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.ImageView);
                }
                else
                {
                    var file = Uri.FromFile(new File(message.Media));
                    Glide.With(MainActivity).Load(file.Path).Apply(Options).Into(holder.ImageView);
                    //Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadFileOfChatItem(Holders.FileViewHolder holder, int position, MessageData message)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (message.Position == "right")
                {
                    SetSeenMessage(holder.Seen, message.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(message.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, message.IsStarted);

                Console.WriteLine(position);
                var fileName = message.Media.Split('/').Last();
                var fileNameWithoutExtension = fileName.Split('.').First();
                var fileNameExtension = fileName.Split('.').Last();

                message.Media = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDcimFile, fileName, message.Media);

                holder.MsgTimeTextView.Text = message.TimeText;
                holder.FileNameTextView.Text = Methods.FunString.SubStringCutOf(fileNameWithoutExtension, 10) + fileNameExtension;
                holder.SizeFileTextView.Text = message.FileSize;

                if (fileNameExtension.Contains("rar") || fileNameExtension.Contains("RAR") || fileNameExtension.Contains("zip") || fileNameExtension.Contains("ZIP"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c6"); //ZipBox
                }
                else if (fileNameExtension.Contains("txt") || fileNameExtension.Contains("TXT"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf15c"); //NoteText
                }
                else if (fileNameExtension.Contains("docx") || fileNameExtension.Contains("DOCX") || fileNameExtension.Contains("doc") || fileNameExtension.Contains("DOC"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c2"); //FileWord
                }
                else if (fileNameExtension.Contains("pdf") || fileNameExtension.Contains("PDF"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c1"); //FilePdf
                }
                else if (fileNameExtension.Contains("apk") || fileNameExtension.Contains("APK"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf17b"); //Fileandroid
                }
                else
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf15b"); //file
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadGifOfChatItem(Holders.ImageViewHolder holder, int position, MessageData item)
        {
            try
            {
                if (holder.UserName != null)
                {
                    holder.UserName.Text = WoWonderTools.GetNameFinal(item.UserData);
                    holder.UserName.Visibility = ViewStates.Visible;
                }

                if (item.Position == "right")
                {
                    SetSeenMessage(holder.Seen, item.Seen);
                    //holder.BubbleLayout.BackgroundTintList = ColorStateList.ValueOf(Color.ParseColor(item.ChatColor));
                }

                //SetStartedMessage(holder.StarIcon, holder.StarImage, item.IsStarted);

                Console.WriteLine(position);
                // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                string imageUrl = "";
                if (!string.IsNullOrEmpty(item.Stickers))
                    imageUrl = item.Stickers;
                else if (!string.IsNullOrEmpty(item.Media))
                    imageUrl = item.Media;
                else if (!string.IsNullOrEmpty(item.MediaFileName))
                    imageUrl = item.MediaFileName;

                string[] fileName = imageUrl.Split(new[] { "/", "200.gif?cid=", "&rid=200" }, StringSplitOptions.RemoveEmptyEntries);
                var lastFileName = fileName.Last();
                var name = fileName[3] + lastFileName;

                item.Media = WoWonderTools.GetFile(GroupId, Methods.Path.FolderDiskGif, name, imageUrl);

                if (item.Media != null && (item.Media.Contains("http")))
                {
                    GlideImageLoader.LoadImage(MainActivity, imageUrl, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    var file = Uri.FromFile(new File(item.Media));
                    Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                    //Glide.With(MainActivity).Load(file.Path).Apply(Options).Into(holder.ImageGifView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //==============================

        public override int ItemCount => DifferList?.Count ?? 0;

        public AdapterModelsClassGroup GetItem(int position)
        {
            var item = DifferList[position];

            return item;
        }

        public override long GetItemId(int position)
        {
            var item = DifferList[position];
            if (item == null)
                return 0;

            return item.Id;
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                var item = DifferList[position];
                if (item == null)
                    return (int)MessageModelType.None;

                switch (item.TypeView)
                {
                    case MessageModelType.RightProduct:
                        return (int)MessageModelType.RightProduct;
                    case MessageModelType.LeftProduct:
                        return (int)MessageModelType.LeftProduct;
                    case MessageModelType.RightGif:
                        return (int)MessageModelType.RightGif;
                    case MessageModelType.LeftGif:
                        return (int)MessageModelType.LeftGif;
                    case MessageModelType.RightText:
                        return (int)MessageModelType.RightText;
                    case MessageModelType.LeftText:
                        return (int)MessageModelType.LeftText;
                    case MessageModelType.RightImage:
                        return (int)MessageModelType.RightImage;
                    case MessageModelType.LeftImage:
                        return (int)MessageModelType.LeftImage;
                    case MessageModelType.RightAudio:
                        return (int)MessageModelType.RightAudio;
                    case MessageModelType.LeftAudio:
                        return (int)MessageModelType.LeftAudio;
                    case MessageModelType.RightContact:
                        return (int)MessageModelType.RightContact;
                    case MessageModelType.LeftContact:
                        return (int)MessageModelType.LeftContact;
                    case MessageModelType.RightVideo:
                        return (int)MessageModelType.RightVideo;
                    case MessageModelType.LeftVideo:
                        return (int)MessageModelType.LeftVideo;
                    case MessageModelType.RightSticker:
                        return (int)MessageModelType.RightSticker;
                    case MessageModelType.LeftSticker:
                        return (int)MessageModelType.LeftSticker;
                    case MessageModelType.RightFile:
                        return (int)MessageModelType.RightFile;
                    case MessageModelType.LeftFile:
                        return (int)MessageModelType.LeftFile;
                    case MessageModelType.RightMap:
                        return (int)MessageModelType.RightMap;
                    case MessageModelType.LeftMap:
                        return (int)MessageModelType.LeftMap;
                    default:
                        return (int)MessageModelType.None;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int)MessageModelType.None;
            }
        }

        void OnClick(Holders.MesClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Holders.MesClickEventArgs args) => ItemLongClick?.Invoke(this, args);


        #region MusicBar

        private void InitValueAnimator(float playbackSpeed, int progress, int max)
        {
            int timeToEnd = (int)((max - progress) / playbackSpeed);
            if (timeToEnd > 0)
            {
                if (ValueAnimator.OfInt(progress, max).SetDuration(timeToEnd) is ValueAnimator value)
                {
                    MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar?.SetProgressAnimator(value);

                    MValueAnimator = value;
                    MValueAnimator.SetInterpolator(new LinearInterpolator());
                    MValueAnimator.AddUpdateListener(this);
                    MValueAnimator.Start();
                }
            }
        }

        //====================== IOnMusicBarAnimationChangeListener ======================
        public void OnHideAnimationEnd()
        {

        }

        public void OnHideAnimationStart()
        {

        }

        public void OnShowAnimationEnd()
        {

        }

        public void OnShowAnimationStart()
        {

        }

        //====================== IOnMusicBarProgressChangeListener ======================

        public void OnProgressChanged(MusicBar musicBarView, int progress, bool fromUser)
        {
            if (fromUser)
                MSeekBarIsTracking = true;
        }

        public void OnStartTrackingTouch(MusicBar musicBarView)
        {
            MSeekBarIsTracking = true;
        }

        public void OnStopTrackingTouch(MusicBar musicBarView)
        {
            try
            {
                MSeekBarIsTracking = false;
                MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar?.InitProgressAnimator(1.0f, musicBarView.GetPosition(), MusicBarViewHolder.MediaPlayer.Duration);
                MusicBarViewHolder?.MediaPlayer?.SeekTo(musicBarView.GetPosition());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            try
            {
                if (MSeekBarIsTracking)
                {
                    animation.Cancel();
                }
                else
                {
                    MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar.SetProgress((int)animation.AnimatedValue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

    }
}