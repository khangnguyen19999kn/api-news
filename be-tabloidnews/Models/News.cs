using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;
using System.Text.RegularExpressions;


namespace be_tabloidnews.Models

{
    public class News
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string id { get; set; }
        [BsonElement("title")]
        public string title { get; set; }
        [BsonElement("slug")]
        [BsonIgnoreIfNull]
        [BsonDefaultValue(null)]
        public string slug { get; set; }
        [BsonElement("content")]
        public string content { get; set; }
        [BsonElement("topic")]
        public string topic { get; set; }

        [BsonElement("slugTopic")]
        public string slugTopic { get; set; }

        [BsonElement("author")]
        public string author { get; set; }
        [BsonElement("tags")]
        public string[] tags { get; set; }
        [BsonElement("bannerImg")]
        public string bannerImg { get; set; }
        [BsonElement("viewCount")]
        [BsonDefaultValue(0)]
        public int viewCount { get; set; }
        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }
        [BsonElement("description")]
        public string description { get; set; }

        public string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var withoutVietnameseChars = RemoveVietnameseChars(input);
            var withoutSpecialChars = Regex.Replace(withoutVietnameseChars, @"[^a-zA-Z0-9]+", "-");
            var slug = withoutSpecialChars.Trim('-').ToLowerInvariant();

            return slug;
        }

        private string RemoveVietnameseChars(string str)
        {
            var fromVietnamese = "áàảãạâấầẩẫậăắằẳẵặéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ";
            var toEnglish = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyydAAAAAAAAAAAAAAAAAEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYYD";

            var stringBuilder = new StringBuilder();
            foreach (var c in str)
            {
                var index = fromVietnamese.IndexOf(c);
                if (index >= 0)
                    stringBuilder.Append(toEnglish[index]);
                else
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

    }

}

