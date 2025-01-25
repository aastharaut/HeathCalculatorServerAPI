using Microsoft.AspNetCore.Mvc;

namespace HealthCalculatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCalculatorController : ControllerBase
    {
        /// <summary>
        /// Calculates BMI. Supports both metric (kg, cm) and imperial (lbs, inches).
        /// </summary>
        [HttpGet("bmi")]
        public IActionResult CalculateBMI(double weight, double height, string unit = "metric", string gender = "female")
        {
            if (weight <= 0 || height <= 0)
                return BadRequest("Weight and height must be greater than zero.");

            double weightKg = weight;
            double heightM = height;

            // Convert units if user provides imperial (lbs, inches)
            if (unit.ToLower() == "imperial")
            {
                weightKg = weight * 0.453592;  // Convert lbs to kg
                heightM = height * 0.0254;     // Convert inches to meters
            }
            else
            {
                heightM = height / 100;       // Convert cm to meters
            }

            double bmi = weightKg / (heightM * heightM);
            string bmiCategory = GetBmiCategory(bmi);
            string bmiRecommendation = GetBmiRecommendation(bmi, gender);

            return Ok(new
            {
                BMI = Math.Round(bmi, 2),
                Category = bmiCategory,
                Recommendation = bmiRecommendation
            });
        }

        /// <summary>
        /// Calculates Body Adiposity Index (BAI). Assumes height in cm and hip circumference in cm.
        /// </summary>
        [HttpGet("bai")]
        public IActionResult CalculateBAI(double hipCircumference, double height, string unit = "metric", string gender = "female")
        {
            if (hipCircumference <= 0 || height <= 0)
                return BadRequest("Hip circumference and height must be greater than zero.");

            double heightM = (unit.ToLower() == "imperial") ? height * 0.0254 : height / 100; // Convert inches to meters if needed

            double bai = (hipCircumference / Math.Pow(heightM, 1.5)) - 18;
            string baiCategory = GetBaiCategory(bai, gender);

            return Ok(new
            {
                BAI = Math.Round(bai, 2),
                Category = baiCategory
            });
        }

        /// <summary>
        /// Calculates Waist-to-Hip Ratio. Supports both cm and inches.
        /// </summary>
        [HttpGet("waisttohip")]
        public IActionResult CalculateWaistToHip(double waist, double hip, string unit = "metric", string gender = "female")
        {
            if (waist <= 0 || hip <= 0)
                return BadRequest("Waist and hip measurements must be greater than zero.");

            // Convert inches to cm if user provides imperial units
            if (unit.ToLower() == "imperial")
            {
                waist *= 2.54; // Inches to cm
                hip *= 2.54;   // Inches to cm
            }

            double ratio = waist / hip;
            string whrCategory = GetWhrCategory(ratio, gender);

            return Ok(new
            {
                WaistToHipRatio = Math.Round(ratio, 2),
                Category = whrCategory
            });
        }

        // BMI Category Logic with gender-based recommendations
        private string GetBmiCategory(double bmi)
        {
            if (bmi < 18.5)
            {
                return "Underweight";
            }
            else if (bmi >= 18.5 && bmi < 24.9)
            {
                return "Healthy weight";
            }
            else if (bmi >= 25 && bmi < 29.9)
            {
                return "Overweight";
            }
            else
            {
                return "Obese";
            }
        }

        // BMI Recommendations based on gender (optional)
        private string GetBmiRecommendation(double bmi, string gender)
        {
            if (bmi < 18.5)
            {
                return (gender.ToLower() == "female") ? "Consider consulting a healthcare provider for nutritional guidance." : "Consult a healthcare provider for advice on weight management.";
            }
            else if (bmi >= 18.5 && bmi < 24.9)
            {
                return "Maintaining a healthy weight is essential for overall health.";
            }
            else if (bmi >= 25 && bmi < 29.9)
            {
                return (gender.ToLower() == "female") ? "Consider a balanced diet and regular exercise to reach a healthy weight." : "Regular physical activity is recommended to reduce health risks.";
            }
            else
            {
                return "Obesity can increase the risk of chronic diseases. A healthcare professional can provide personalized recommendations.";
            }
        }

        // BAI Category Logic with gender consideration
        private string GetBaiCategory(double bai, string gender)
        {
            if (gender.ToLower() == "female")
            {
                if (bai < 18)
                {
                    return "Underfat";
                }
                else if (bai >= 18 && bai < 25)
                {
                    return "Normal";
                }
                else
                {
                    return "Overfat";
                }
            }
            else // for male
            {
                if (bai < 8)
                {
                    return "Underfat";
                }
                else if (bai >= 8 && bai < 21)
                {
                    return "Normal";
                }
                else
                {
                    return "Overfat";
                }
            }
        }

        // WHR Category Logic with gender consideration
        private string GetWhrCategory(double ratio, string gender)
        {
            if (gender.ToLower() == "female")
            {
                if (ratio < 0.80)
                {
                    return "Low Risk";
                }
                else if (ratio >= 0.80 && ratio < 0.85)
                {
                    return "Moderate Risk";
                }
                else
                {
                    return "High Risk";
                }
            }
            else // for male
            {
                if (ratio < 0.90)
                {
                    return "Low Risk";
                }
                else if (ratio >= 0.90 && ratio < 0.95)
                {
                    return "Moderate Risk";
                }
                else
                {
                    return "High Risk";
                }
            }
        }
    }
}