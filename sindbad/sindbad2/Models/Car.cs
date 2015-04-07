using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace sindbad2.Models
{
    public enum carType
    {
        STANDARD, TWO_DOOR, TWO_OR_FOUR_DOOR, FOUR_OR_FIVE_DOOR,
        ELITE, COUPE_OR_ROADSTER, PICKUP, ESTATE_OR_WAGON,
        RECREATIONAL_VEHICLE, SUV, PASSENGER_VAN,
        SIX_PERSON_VAN, SEVEN_PERSON_VAN, EIGHT_PERSON_VAN,
        NINE_PERSON_VAN, AWD, ALL_TERRAIN, COMMERCIAL_TRUCK,
        LIMOUSINE, SPORT, CONVERTIBLE, SPECIAL_OFFER_CAR, MONOSPACE,
        MOTOR_HOME, MOTORCYCLE, CROSSOVER, MANUAL_TRANSMISSION,
        AUTOMATIC_TRANSMISSION, PETROL_POWERED, DIESEL_POWERED,
        LOWER_EMISSION, HYBRID, ELECTRIC_POWERED, HYDROGEN_POWERED,
        MULTI_FUEL_POWERED, LPG_OR_COMPRESSED_GAS_POWERED, ETHANOL_POWERED
    }
    public enum ratePlan
    {
         DAILY, WEEKEND, WEEKLY, MONTHLY
    }
    public class Car
    {
        private string providerName;
        private double price1;
        private ratePlan type1;
        private Url imageUrl1;
        private double estimatedTotal1;
        private string line1;
        private string city { get; set; }

        public Car(double price1, ratePlan type1, Url imageUrl1, double estimatedTotal1, string providerName, string line1, string city)
        {
            // TODO: Complete member initialization
            this.price1 = price1;
            this.type1 = type1;
            this.imageUrl1 = imageUrl1;
            this.estimatedTotal1 = estimatedTotal1;
            this.providerName = providerName;
            this.line1 = line1;
            this.city = city;
        }

        public override string ToString()
        {
            return providerName;
        }
    }
}
