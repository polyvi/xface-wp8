using System;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using xFaceLib.runtime;
using System.Collections.Generic;

namespace xFaceLib
{
    public partial class xFacePage : PhoneApplicationPage
    {

        /// <summary>
        /// xFace 管理应用运行的runtime
        /// </summary>
        private XRuntime runtime;

        public xFacePage()
        {
            InitializeComponent();

            //runtime的初始化
            this.runtime = new XRuntime(this.LayoutRoot);
            //backPress
            this.BackKeyPress += this.runtime.HandleBackKeyPress;
            //xFacePage Loaded
            this.Loaded += this.runtime.PageLoaded;
        }

        //记录用户的页面访问情况和页面停留时间
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UmengSDK.UmengAnalytics.onPageEnd("xFacePage");
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<string, string> queryStrings = NavigationContext.QueryString;

            if (queryStrings.ContainsKey("Msg"))
            {
                var value = NavigationContext.QueryString["Msg"];
                //todo:startparams的解析与保存

            }
            base.OnNavigatedTo(e);
            UmengSDK.UmengAnalytics.onPageStart("xFacePage");
        }
    }
}
