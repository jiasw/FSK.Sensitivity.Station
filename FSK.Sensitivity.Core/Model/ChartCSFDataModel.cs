using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class ChartCSFDataModel : BindableBase
    {
        public Eye Eye { get; set; }

        public LightStatus DazzleLight { get; set; }

        public bool IsDay { get; set; }


        private List<CSFPointModel> _microspurPoints = new List<CSFPointModel>();
        private List<CSFPointModel> _shortRangePoints = new List<CSFPointModel>();
        private List<CSFPointModel> _midrangePoints = new List<CSFPointModel>();
        private List<CSFPointModel> _longRangePoints = new List<CSFPointModel>();
        private string _title;

        public ChartCSFDataModel()
        {
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public List<CSFPointModel> MicrospurPoints
        {
            get { return _microspurPoints; }
            set { SetProperty(ref _microspurPoints, value); }
        }


        public void RefrushPoints()
        {
            RaisePropertyChanged("MicrospurPoints");
        }

        public List<CSFPointModel> ShortRangePoints
        {
            get { return _shortRangePoints; }
            set { SetProperty(ref _shortRangePoints, value); }
        }

        public List<CSFPointModel> MidrangePoints
        {
            get { return _midrangePoints; }
            set { SetProperty(ref _midrangePoints, value); }
        }

        public List<CSFPointModel> LongRangePoints
        {
            get { return _longRangePoints; }
            set { SetProperty(ref _longRangePoints, value); }

        }

    }
}
