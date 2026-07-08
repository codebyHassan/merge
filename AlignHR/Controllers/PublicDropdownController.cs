using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AlignHR.Controllers
{
    [Route("api/dropdown")]
    public class PublicDropdownController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public PublicDropdownController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ── Static embedded data ────────────────────────────────────────────────

        private static readonly string[] _countries = new[]
        {
            "Afghanistan","Albania","Algeria","Argentina","Australia","Austria","Azerbaijan",
            "Bahrain","Bangladesh","Belarus","Belgium","Bolivia","Brazil","Bulgaria",
            "Cambodia","Canada","Chile","China","Colombia","Croatia","Czech Republic",
            "Denmark","Ecuador","Egypt","Ethiopia","Finland","France","Georgia","Germany",
            "Ghana","Greece","Hungary","India","Indonesia","Iran","Iraq","Ireland","Israel",
            "Italy","Japan","Jordan","Kazakhstan","Kenya","Kuwait","Lebanon","Libya",
            "Malaysia","Mexico","Morocco","Myanmar","Nepal","Netherlands","New Zealand",
            "Nigeria","Norway","Oman","Pakistan","Peru","Philippines","Poland","Portugal",
            "Qatar","Romania","Russia","Saudi Arabia","Serbia","Singapore","South Africa",
            "South Korea","Spain","Sri Lanka","Sudan","Sweden","Switzerland","Syria",
            "Taiwan","Tanzania","Thailand","Tunisia","Turkey","UAE","Uganda","Ukraine",
            "United Kingdom","United States","Uzbekistan","Venezuela","Vietnam","Yemen","Zimbabwe"
        };

        private static readonly Dictionary<string, string[]> _staticUniversities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Pakistan"] = new[]
            {
                "Aga Khan University (AKU)",
                "Air University",
                "Allama Iqbal Open University (AIOU)",
                "Army Medical College",
                "Bahauddin Zakariya University",
                "Bahria University",
                "BUITEMS - Balochistan University of IT, Engineering & Management Sciences",
                "Capital University of Science & Technology (CUST)",
                "COMSATS University Islamabad",
                "Dawood University of Engineering & Technology",
                "DHA Suffa University",
                "Dow University of Health Sciences",
                "FAST - National University of Computer & Emerging Sciences (NUCES)",
                "Fatima Jinnah Women University",
                "Federal Urdu University",
                "Forman Christian College University",
                "Foundation University",
                "GC University Faisalabad",
                "GC University Lahore",
                "Ghulam Ishaq Khan Institute (GIKI)",
                "Habib University",
                "Hamdard University",
                "IBA Karachi - Institute of Business Administration",
                "Institute of Business Management (IoBM)",
                "International Islamic University Islamabad (IIUI)",
                "Isra University",
                "Jinnah Sindh Medical University",
                "Karachi University (University of Karachi)",
                "KEMU - King Edward Medical University",
                "Khyber Medical University",
                "Lahore College for Women University",
                "Lahore University of Management Sciences (LUMS)",
                "Liaquat University of Medical & Health Sciences",
                "Mehran University of Engineering & Technology",
                "Minhaj University Lahore",
                "Mohammad Ali Jinnah University",
                "NED University of Engineering & Technology",
                "NUST - National University of Sciences & Technology",
                "Pak-Austria Fachhochschule (PAF)",
                "PAF-KIET",
                "Pakistan Institute of Engineering & Applied Sciences (PIEAS)",
                "Punjab University (University of the Punjab)",
                "Quaid-i-Azam University",
                "Riphah International University",
                "Shaheed Zulfiqar Ali Bhutto Medical University",
                "Shaheed Zulfiqar Ali Bhutto University of Law",
                "Sindh University",
                "Sir Syed University of Engineering & Technology",
                "SZABIST",
                "University of Agriculture Faisalabad",
                "University of Central Punjab (UCP)",
                "University of Education Lahore",
                "University of Engineering & Technology Lahore (UET)",
                "University of Engineering & Technology Peshawar (UET)",
                "University of Engineering & Technology Taxila (UET)",
                "University of Faisalabad",
                "University of Gujrat",
                "University of Health Sciences Lahore",
                "University of Management & Technology (UMT)",
                "University of Peshawar",
                "University of Sargodha",
                "University of Sindh",
                "University of South Asia",
                "University of Veterinary & Animal Sciences",
                "University of Wah",
                "Virtual University of Pakistan",
                "Ziauddin University",
                "Other / Foreign University"
            },
            ["India"] = new[]
            {
                "IIT Bombay","IIT Delhi","IIT Madras","IIT Kanpur","IIT Kharagpur",
                "IIM Ahmedabad","IIM Bangalore","IIM Calcutta","BITS Pilani",
                "University of Delhi","University of Mumbai","Anna University",
                "Jadavpur University","Osmania University","Jamia Millia Islamia",
                "Aligarh Muslim University","Banaras Hindu University",
                "Calcutta University","Pune University","Hyderabad University",
                "NIT Trichy","NIT Surathkal","SRCC Delhi","St. Xavier's College",
                "Symbiosis International University","Manipal University",
                "Amity University","VIT University","SRM University",
                "Other / Foreign University"
            },
            ["United Kingdom"] = new[]
            {
                "University of Oxford","University of Cambridge","Imperial College London",
                "UCL (University College London)","London School of Economics (LSE)",
                "University of Edinburgh","King's College London","University of Manchester",
                "University of Bristol","University of Warwick","University of Leeds",
                "University of Sheffield","University of Birmingham","University of Glasgow",
                "University of Nottingham","University of Southampton","University of Liverpool",
                "Durham University","University of Bath","Lancaster University",
                "University of Aberdeen","Heriot-Watt University","Loughborough University",
                "University of Reading","University of Surrey","Brunel University",
                "Cardiff University","Queen's University Belfast","University of Strathclyde",
                "Other / Foreign University"
            },
            ["United States"] = new[]
            {
                "Harvard University","Stanford University","MIT","California Institute of Technology (Caltech)",
                "University of Chicago","Princeton University","Yale University","Columbia University",
                "University of Pennsylvania","Cornell University","Johns Hopkins University",
                "Northwestern University","Duke University","Dartmouth College","Brown University",
                "Vanderbilt University","Rice University","Washington University in St. Louis",
                "Notre Dame University","Georgetown University","Emory University",
                "UC Berkeley","UCLA","University of Michigan","University of Virginia",
                "University of North Carolina","New York University (NYU)","Boston University",
                "Purdue University","Penn State University","Ohio State University",
                "University of Texas at Austin","University of Washington","Georgia Tech",
                "Other / Foreign University"
            },
            ["UAE"] = new[]
            {
                "University of Dubai","American University of Sharjah","American University in Dubai",
                "UAE University","Zayed University","Abu Dhabi University","Khalifa University",
                "University of Sharjah","Ajman University","British University in Dubai",
                "Middlesex University Dubai","Heriot-Watt University Dubai","Rochester Institute of Technology Dubai",
                "Canadian University Dubai","Paris Sorbonne University Abu Dhabi",
                "Other / Foreign University"
            },
            ["Saudi Arabia"] = new[]
            {
                "King Abdulaziz University","King Fahd University of Petroleum & Minerals",
                "King Saud University","King Abdullah University of Science & Technology (KAUST)",
                "Prince Sultan University","Effat University","Imam Muhammad ibn Saud Islamic University",
                "Taibah University","Qassim University","Taif University",
                "Other / Foreign University"
            },
        };

        private static readonly Dictionary<string, string[]> _staticCities = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Pakistan"] = new[]
            {
                "Abbottabad","Attock","Bahawalpur","Chakwal","Chiniot","Dadu","Dera Ghazi Khan",
                "Dera Ismail Khan","Faisalabad","Ghotki","Gujranwala","Gujrat","Hafizabad",
                "Haripur","Hyderabad","Islamabad","Jacobabad","Jhang","Jhelum","Karachi",
                "Kasur","Khanewal","Khushab","Kohat","Lahore","Larkana","Layyah","Lodhran",
                "Mandi Bahauddin","Mansehra","Mardan","Mirpur Khas","Multan","Muzaffarabad",
                "Muzaffargarh","Narowal","Nawabshah","Nowshera","Okara","Pakpattan",
                "Peshawar","Quetta","Rahim Yar Khan","Rawalpindi","Sahiwal","Sargodha",
                "Sheikhupura","Sialkot","Sibi","Sukkur","Swabi","Swat","Toba Tek Singh",
                "Vehari","Wah Cantonment","Other"
            }
        };

        // ── Endpoints ────────────────────────────────────────────────────────────

        // GET /api/dropdown/countries
        [HttpGet("countries")]
        public IActionResult GetCountries() => Ok(_countries);

        // GET /api/dropdown/cities?country=Pakistan
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return Ok(Array.Empty<string>());

            // Return static list if available
            if (_staticCities.TryGetValue(country, out var staticList))
                return Ok(staticList);

            // Otherwise proxy CountriesNow
            try
            {
                var client = _httpClientFactory.CreateClient("DropdownApi");
                var payload = JsonSerializer.Serialize(new { country });
                var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://countriesnow.space/api/v0.1/countries/cities", content);

                if (!response.IsSuccessStatusCode)
                    return Ok(new[] { "Other" });

                var body = await response.Content.ReadAsStringAsync();
                var node = JsonNode.Parse(body);

                if (node?["error"]?.GetValue<bool>() == true)
                    return Ok(new[] { "Other" });

                var data = node?["data"]?.AsArray();
                if (data == null || data.Count == 0)
                    return Ok(new[] { "Other" });

                var cities = data
                    .Select(c => c?.GetValue<string>())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .OrderBy(c => c)
                    .Append("Other")
                    .ToArray();

                return Ok(cities);
            }
            catch
            {
                return Ok(new[] { "Other" });
            }
        }

        // GET /api/dropdown/universities?country=Pakistan
        [HttpGet("universities")]
        public async Task<IActionResult> GetUniversities([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return Ok(Array.Empty<string>());

            // Return static list if available (fast, no API call)
            if (_staticUniversities.TryGetValue(country, out var staticList))
                return Ok(staticList);

            // Otherwise try Hipolabs
            try
            {
                var client = _httpClientFactory.CreateClient("DropdownApi");
                var url = $"https://universities.hipolabs.com/search?country={Uri.EscapeDataString(country)}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return Ok(new[] { "Other / Foreign University" });

                var body = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<HipolabsUniversity>>(body, _json);

                if (items == null || items.Count == 0)
                    return Ok(new[] { "Other / Foreign University" });

                var names = items
                    .Where(u => !string.IsNullOrWhiteSpace(u.Name))
                    .Select(u => u.Name!)
                    .Distinct()
                    .OrderBy(n => n)
                    .Append("Other / Foreign University")
                    .ToArray();

                return Ok(names);
            }
            catch
            {
                return Ok(new[] { "Other / Foreign University" });
            }
        }

        private class HipolabsUniversity
        {
            public string? Name { get; set; }
            public string? Country { get; set; }
        }
    }
}
