using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        private double? _temperatureC;
        private double? _temperatureF;
        public double? TemperatureC { 
            get 
            {
                if (_temperatureC.HasValue) return _temperatureC.Value;
                if (_temperatureF.HasValue) return (_temperatureF.Value - 32) * 5 / 9;
                return null;
            } 
            set 
            { 
                _temperatureC = value; 
            } 
        }
        public double? TemperatureF { 
            get
            {
                if(_temperatureF.HasValue) return _temperatureF.Value;
                if(_temperatureC.HasValue) return _temperatureC.Value * 9 / 5 + 32;
                return null;
            }
            set 
            { 
                _temperatureF = value;
            } 
        }
        public string Summary { get; set; }
        public string WindSpeed { get; set; }
        public int ChanceOfRain { get; set; }
        public string Name { get; set; }
    }
}
