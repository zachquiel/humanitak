using System;
using System.Linq;
using SmartAdminMvc.ViewModels;

namespace SmartAdminMvc.Helpers {
    public class DeductionHelper {
        public static double[][] ChargeTable = {
                new[] {0.01, 16.32, 0, 1.92},
                new[] {16.33, 138.5, 0.31, 6.4},
                new[] {138.51, 243.4, 8.13, 10.88},
                new[] {243.41, 282.94, 19.55, 16},
                new[] {282.95, 338.76, 25.87, 17.92},
                new[] {338.77, 683.23, 35.88, 21.36},
                new[] {683.24, 1076.87, 109.45, 23.52},
                new[] {1076.88, 2055.92, 202.04, 30},
                new[] {2055.93, 2741.23, 495.75, 32},
                new[] {2741.24, 8223.68, 715.05, 34},
                new[] {8223.69, double.MaxValue, 2579.09, 35},
            };
        public static double[][] SubsidyTable = {
                new[] {0.01, 58.19, 13.39},
                new[] {58.2, 87.28, 13.38},
                new[] {87.29, 114.24, 13.38},
                new[] {114.25, 116.38, 12.92},
                new[] {116.39, 146.25, 12.58},
                new[] {146.26, 155.17, 11.65},
                new[] {155.18, 175.51, 10.69},
                new[] {175.52, 204.76, 9.69},
                new[] {204.77, 234.01, 8.34},
                new[] {234.02, 242.84, 7.16},
                new[] {242.85, double.MaxValue, 0}
            };
        public static double CalculateIsrFixed(double totalAmount, int totalDays) {
            var tableVal = ChargeTable.First(c => c[0] * totalDays <= totalAmount && c[1] * totalDays >= totalAmount);
            return tableVal[2] * totalDays;
        }

        public static double CalculateIsrPct(double totalAmount, int totalDays) {
            var tableVal = ChargeTable.First(c => c[0] * totalDays <= totalAmount && c[1] * totalDays >= totalAmount);
            var minLim = tableVal[0] * totalDays;
            var baseVal = totalAmount - minLim;
            var ratio = tableVal[3];
            return baseVal * (ratio / 100);
        }

        public static double CalculateIsrSubsidy(double totalAmount, int totalDays) {
            var subsidyVal = SubsidyTable.First(c => c[0] * totalDays <= totalAmount && c[1] * totalDays >= totalAmount);
            return subsidyVal[2] * totalDays;
        }

        public static double CalculateIsr(double totalAmount, int totalDays) {
            var deduction = 0d;
            deduction += CalculateIsrFixed(totalAmount, totalDays);
            deduction += CalculateIsrPct(totalAmount, totalDays);
            deduction -= CalculateIsrSubsidy(totalAmount, totalDays);
            return totalAmount - deduction;
        }

        public static double CalculateIsrDeduction(double totalAmount, int totalDays) {
            var deduction = 0d;
            deduction += CalculateIsrFixed(totalAmount, totalDays);
            deduction += CalculateIsrPct(totalAmount, totalDays);
            deduction -= CalculateIsrSubsidy(totalAmount, totalDays);
            return deduction;
        }

        //public static double CalculateIsr(double totalAmount, int totalDays) {
        //    var deduction = 0d;
        //    deduction += CalculateIsrFixed(totalAmount, totalDays);
        //    deduction += CalculateIsrPct(totalAmount, totalDays);
        //    deduction -= CalculateIsrSubsidy(totalAmount, totalDays);
        //    return deduction;
        //}

        public static double CalculateImss(double dailyAmount, bool evenMonth = true) {
            var bimonthlyBeforeImss = dailyAmount * 60.8332;
            var imss = 0d;
            if (dailyAmount >= 2001)
                imss = evenMonth ? 785.20 : 2174.01;
            if (dailyAmount <= 80.04)
                imss = 0;
            if (imss <= 0) {
                var fullBimonthlyImss = ((bimonthlyBeforeImss * 100) / 97.625);
                var fullMonthlyImss = fullBimonthlyImss / 2;
                var montlyImss = fullMonthlyImss * 0.0125;
                var bimontlyImss = fullBimonthlyImss * 0.01125;
                imss = evenMonth ? montlyImss : bimontlyImss;
            }
            return imss;
        }

        public static double DeductImss(double dailyAmount, bool evenMonth = true) {
            var monthlyBeforeImss = dailyAmount * 30.4166;
            var bimonthlyBeforeImss = dailyAmount * 60.8332;
            var imss = 0d;
            if (dailyAmount >= 2001)
                imss = evenMonth ? 785.20 : 2174.01;
            if (dailyAmount <= 80.04)
                imss = 0;
            if (imss <= 0) {
                var montlyImss = monthlyBeforeImss * 0.0125;
                var bimontlyImss = bimonthlyBeforeImss * 0.01125;
                imss = evenMonth ? montlyImss : bimontlyImss;
            }
            return imss;
        }

        public static double CalculateDeductions(double dailyAmount, int totalDays, bool hasSocialSecurity, bool evenMonth = true) {
            var multiplier = 30.4166;
            var factor = 1;
            if (totalDays == 15) factor = 2;
            else if (totalDays == 10) factor = 3;
            else if (totalDays == 7) factor = 4;
            else if (totalDays == 1) factor = 30;
            multiplier = multiplier / factor;

            var imssDeduction = 0d;
            if (hasSocialSecurity) {
                imssDeduction = DeductImss(dailyAmount);
                imssDeduction += (DeductImss(dailyAmount, !evenMonth) / 2);
                if (totalDays == 1) imssDeduction = imssDeduction / 30;
                else if (totalDays == 7) imssDeduction = imssDeduction / 4;
                else if (totalDays == 10) imssDeduction = imssDeduction / 3;
                else if (totalDays == 15) imssDeduction = imssDeduction / 2;
            }
            var isrDeduction = CalculateIsrDeduction((dailyAmount * multiplier) - imssDeduction, totalDays);
            return isrDeduction + imssDeduction;
        }

        public static SalaryComponentsViewModel GetCalculatedSalaryValues(double input, int totalDays, bool hasImss = true) {
            var isr = CalculateIsr(input, totalDays);
            var newInput = input;
            while (Math.Abs(isr-input) > 0.0000001) {
                newInput = input - isr + newInput;
                isr = CalculateIsr(newInput, totalDays);
            }

            var divider = 30.4166;
            var factor = 1;
            if (totalDays == 15) factor = 2;
            else if (totalDays == 10) factor = 3;
            else if (totalDays == 7) factor = 4;
            else if (totalDays == 1) factor = 30;
            divider = divider / factor;

            var dailyBeforeImss = newInput / divider;

            var monthlyImss = hasImss ? CalculateImss(dailyBeforeImss) : 0;
            var bimonthlyImss = hasImss ? CalculateImss(dailyBeforeImss, false) : 0;
            //imss = imss / divider;
            var proratedMonthlyImss = hasImss ? monthlyImss / factor : 0;
            var proratedBimonthlyImss = hasImss ? (bimonthlyImss / 2) / factor : 0;
            var proratedImss = hasImss ? proratedMonthlyImss + proratedBimonthlyImss : 0;


            var res = new SalaryComponentsViewModel {
                FixedIsrDeduction = CalculateIsrFixed(newInput, totalDays),
                PctIsrDeduction = CalculateIsrPct(newInput, totalDays),
                IsrDeductionSubsidy = CalculateIsrSubsidy(newInput, totalDays),
                Divider = divider,
                MonthlyImss = monthlyImss,
                BimonthlyImss = bimonthlyImss,
                GrossSalary = newInput + proratedImss,
                ProratedMonthlyImss = monthlyImss / factor,
                ProratedBimonthlyImss = (bimonthlyImss / 2) / factor
            };

            return res;
        }

        public static SalaryComponentsViewModel GetStraightCalculatedSalaryValues(double input, int totalDays, bool hasImss = true) {
            //var isr = CalculateIsr(input, totalDays);
            var newInput = input;
            //while (Math.Abs(isr-input) > 0.0000001) {
              //  newInput = input - isr + newInput;
                //isr = CalculateIsr(newInput, totalDays);
            //}

            var divider = 30.4166;
            var factor = 1;
            if (totalDays == 15) factor = 2;
            else if (totalDays == 10) factor = 3;
            else if (totalDays == 7) factor = 4;
            else if (totalDays == 1) factor = 30;
            divider = divider / factor;

            var dailyBeforeImss = newInput / divider;

            var monthlyImss = hasImss ? CalculateImss(dailyBeforeImss) : 0;
            var bimonthlyImss = hasImss ? CalculateImss(dailyBeforeImss, false) : 0;
            //imss = imss / divider;
            var proratedMonthlyImss = hasImss ? monthlyImss / factor : 0;
            var proratedBimonthlyImss = hasImss ? (bimonthlyImss / 2) / factor : 0;
            var proratedImss = hasImss ? proratedMonthlyImss + proratedBimonthlyImss : 0;


            var res = new SalaryComponentsViewModel {
                FixedIsrDeduction = CalculateIsrFixed(newInput, totalDays),
                PctIsrDeduction = CalculateIsrPct(newInput, totalDays),
                IsrDeductionSubsidy = CalculateIsrSubsidy(newInput, totalDays),
                Divider = divider,
                MonthlyImss = monthlyImss,
                BimonthlyImss = bimonthlyImss,
                GrossSalary = newInput,
                ProratedMonthlyImss = monthlyImss / factor,
                ProratedBimonthlyImss = (bimonthlyImss / 2) / factor
            };

            return res;
        }

        public static double CalculateSalary(double input, int totalDays, bool hasImss = true) {
            var result = GetCalculatedSalaryValues(input, totalDays, hasImss);
            return result.DailySalary;
        }
    }
}