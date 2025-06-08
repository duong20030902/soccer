using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Soccer.Business_Logic.DTO
{
    public class FieldSearchResultDto
    {
        public int ScheduleId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = null!;
        public List<string> ImageUrls { get; set; } = new();
        [JsonConverter(typeof(StringToIntConverter))]
        public int TimeslotId { get; set; }
    }

    public class FieldSearchRequest
    {
        [Required(ErrorMessage = "Ngày là bắt buộc")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Khung giờ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Khung giờ phải lớn hơn 0")]
        public int TimeslotId { get; set; }

        public string? FieldName { get; set; }
    }

    public class FieldDetailDto
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public string Size { get; set; } = null!;
        public string GrassType { get; set; } = null!;
        public string Lighting { get; set; } = null!;
        public string Quality { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<string> Images { get; set; } = new();
        public string MainImage { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public List<string> Facilities { get; set; } = new();
    }

    public class StringToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (int.TryParse(stringValue, out int value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            return 0; // hoặc throw exception tùy theo yêu cầu
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
