﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Entities.RobinhoodAirlineQtaxControl
{
    public class AirlineQtaxControlResponse
    {
        public bool isError { get; set; }
        public string errorMessage { get; set; }
        public List<DataModel.AirlineQtaxControl> airlineQtaxControls { get; set; }
    }
}
