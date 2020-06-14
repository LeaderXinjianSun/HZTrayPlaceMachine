using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HZTrayPlaceMachine.ViewModels
{
    class Point : NotificationObject
    {

        private double x;

        public double X
        {
            get { return x; }
            set
            {
                x = value;
                this.RaisePropertyChanged("X");
            }
        }
        private double y;

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                this.RaisePropertyChanged("Y");
            }
        }
        private double u;

        public double U
        {
            get { return u; }
            set
            {
                u = value;
                this.RaisePropertyChanged("U");
            }
        }

    }
}
