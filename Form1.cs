using Dynamsoft.TWAIN;
using Dynamsoft.TWAIN.Enums;
using Dynamsoft.TWAIN.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dynamsoft
{
    public partial class Form1 : Form, IAcquireCallback
    {
        private TwainManager twain = null;
        private string m_StrProductKey = "t0068MgAAAGb6rOq4pq+p1k2IfqBH43pALaGqfZ04JvYdp8urfgJPLc2v7Faact3eo05bSn8Pdw8XCLr0DMGF439wA7lx3pE=";
        public Form1()
        {
            InitializeComponent();
            var productKey = m_StrProductKey + ";" + LicenseLoader.ReadLocalLicense();
            twain = new TwainManager(productKey);
        }

        public bool IfGetImageInfo => true;

        public bool IfGetExtImageInfo => true;

        public void OnPostAllTransfers()
        {
            twain.CloseSource();
        }

        public bool OnPostTransfer(Bitmap bit, string info)
        {
            bit.Save("d:\\temp\\ScannedImage.tiff", ImageFormat.Tiff);
            return true;
        }

        public void OnPreAllTransfers()
        {

        }

        public bool OnPreTransfer()
        {
            return true;
        }

        public void OnSourceUIClose()
        {

        }

        public void OnTransferCancelled()
        {
            var a = "";
        }

        public void OnTransferError()
        {
            var a = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // twain.SelectSource();
            // twain.CloseSource();

            try
            {
                twain.OpenSource();
                //twain.LogLevel = 1;

                //var setup = SetCapability(twain, TWCapability.ICAP_UNITS, (double)TWICapUNits.TWUN_PIXELS);
                //twain.ExtendedImageInfoQueryLevel = 2;
                var setup = twain.SetFileXFERInfo("d:\\temp\\ScannedImage.tiff", TWICapFileFormat.TWFF_TIFF);
                twain.TransferMode = TWICapSetupXFer.TWSX_MEMORY;
                twain.PixelType = TWICapPixelType.TWPT_RGB;
                twain.BitDepth = 48;
                twain.Unit = TWICapUNits.TWUN_PIXELS;
                //setup = SetCapability(twain, TWCapability.ICAP_BITDEPTH, 48);
                twain.Resolution = 100;
                //GetCapability(twain, TWCapability.ICAP_XRESOLUTION);

                if (true)
                {
                    twain.IfShowUI = false;
                    //twain.IfDuplexEnabled = false;
                    //twain.IfDisableSourceAfterAcquire = true;
                    //twain.IfShowIndicator = false;
                    twain.AcquireImage(this as IAcquireCallback);
                }
            }
            finally
            {
                twain.CloseSource();
                twain.CloseSourceManager();
            }
        }
        //twain.Capability = TWAIN.Enums.TWCapability.ICAP_XRESOLUTION;
        //var didWeGetCap = twain.CapGet();
        //twain.CapType = TWCapType.TWON_RANGE;
        //if (didWeGetCap)
        //{
        //    var capCurrentIndex = twain.CapCurrentIndex;
        //    var currentValue = twain.GetCapItems(capCurrentIndex);
        //    twain.SetCapItems(capCurrentIndex, 1000);
        //    twain.CapSet();
        //}


        //twain.Capability = TWAIN.Enums.TWCapability.ICAP_YRESOLUTION;
        //didWeGetCap = twain.CapGet();
        //if (didWeGetCap)
        //{
        //    twain.CapType = TWCapType.TWON_RANGE;
        //    var capCurrentIndex = twain.CapCurrentIndex;
        //    var currentValue = twain.GetCapItems(capCurrentIndex);
        //    twain.SetCapItems(capCurrentIndex, 1000);
        //}

        //twain.IfShowUI = false;
        //twain.IfShowIndicator = false;
        //twain.AcquireImage(this as IAcquireCallback);
        //twain.Capability = TWAIN.Enums.TWCapability.ICAP_XRESOLUTION;
        //twain.CapValue = 1000;
        //twain.CapType = TWAIN.Enums.TWCapType.TWON_RANGE;
        //twain.CapSet();

        //twain.Capability = TWAIN.Enums.TWCapability.ICAP_YRESOLUTION;
        //result = twain.CapGet();
        //value = twain.CapValue;

        //twain.Capability = TWAIN.Enums.TWCapability.ICAP_YRESOLUTION;
        //twain.CapValue = 1000;
        //twain.CapType = TWAIN.Enums.TWCapType.TWON_RANGE;
        //twain.CapSet();
        private bool SetCapability(TwainManager twain, TWCapability capability, double value)
        {
            var logInfoPrefix = $"{MethodInfo.GetCurrentMethod().Name}->";
            Debug.WriteLine($"{logInfoPrefix} capability:{capability}, value:{value}");
            twain.Capability = capability;
            bool didWeGetCapability = twain.CapGet();
            if (!didWeGetCapability)
            {
                Debug.WriteLine($"{logInfoPrefix}failed to get capability");
                return false;
            }

            Debug.WriteLine($"{logInfoPrefix}capability retrieved, capType:{twain.CapType}");
            switch (twain.CapType)
            {
                case TWCapType.TWON_RANGE:
                    Debug.WriteLine($"{logInfoPrefix}twain.CapCurrentValue");
                    twain.CapCurrentValue = value;
                    break;
                case TWCapType.TWON_ENUMERATION:
                case TWCapType.TWON_ARRAY:
                    Debug.WriteLine($"{logInfoPrefix}twain.SetCapItems->result:{twain.SetCapItems(twain.CapCurrentIndex, value)}");
                    break;
                default:
                    break;
            }
            var setResult = twain.CapSet();
            Debug.WriteLine($"{logInfoPrefix}Set result for {capability} : {setResult}");
            return setResult;
        }

        private void GetCapability(TwainManager twain, TWCapability capability)
        {
            twain.Capability = capability;
            bool result = twain.CapGet();
            if (result)
            {
                var currentType = twain.CapType;
                Debug.Write($"The current value for capability:{capability}, type {currentType}, TWQC_SET:{twain.CapIfSupported(TWQC.TWQC_SET)}, TWQC_RESET:{twain.CapIfSupported(TWQC.TWQC_RESET)}");
                switch (currentType)
                {
                    case TWCapType.TWON_RANGE:
                        Debug.Write($"CapMinValue {twain.CapMinValue}," +
                            $"CapMaxValue:{twain.CapMaxValue}, CapStepSize:{twain.CapStepSize}, CapDefaultValue:{twain.CapDefaultValue}, CapCurrentValue:{twain.CapCurrentValue}");
                        break;
                    case TWCapType.TWON_ONEVALUE:
                        Debug.WriteLine($"CapValue: {twain.CapValue}" +
                            $"CapValue:{twain.CapValue}, CapValueString:{twain.CapValueString}");
                        break;
                    case TWCapType.TWON_ENUMERATION:
                        Debug.WriteLine($"CapNumItems {twain.CapNumItems}," +
                            $"GetCapItems:{twain.GetCapItems(twain.CapCurrentIndex)}, GetCapItemsString:{twain.GetCapItemsString(twain.CapCurrentIndex)}," +
                            $"CapCurrentIndex:{twain.CapCurrentIndex}, CapDefaultIndex:{twain.CapDefaultIndex}");
                        break;
                    case TWCapType.TWON_ARRAY:
                        Debug.WriteLine($"CapNumItems {twain.CapNumItems}," +
                            $"GetCapItems:{twain.GetCapItems(twain.CapCurrentIndex)}, GetCapItemsString:{twain.GetCapItemsString(twain.CapCurrentIndex)}");
                        break;
                }
                Debug.WriteLine("");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                twain.SelectSourceByIndex(0);
                twain.OpenSource();
                var b = sender as Button;
                GetCapability(twain, TWCapability.ICAP_BITDEPTH);
            }
            finally
            {
                twain.CloseSource();
            }
        }
    }

    
}
