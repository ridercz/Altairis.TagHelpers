using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {
    public class GravatarTagHelper : TagHelper {
        private const int DEFAULT_SIZE = 80;
        private const GravatarRating DEFAULT_RATING = GravatarRating.G;

        public string Email { get; set; }

        public int Size { get; set; } = DEFAULT_SIZE;

        public string DefaultImage { get; set; }

        public GravatarRating Rating { get; set; } = DEFAULT_RATING;

        public bool ForceDefault { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;

            output.Attributes.Add("src", this.GetGravatarUrl());
            output.Attributes.Add("width", this.Size.ToString());
            output.Attributes.Add("height", this.Size.ToString());

            if (context.AllAttributes["alt"] == null) output.Attributes.Add("alt", "Gravatar");
        }

        private string GetGravatarUrl() {
            // Construct base URL
            var sb = new StringBuilder();
            sb.Append($"https://www.gravatar.com/avatar/{this.GetEmailHash()}?");

            // Add optional parameters
            if (this.Size != DEFAULT_SIZE) sb.Append($"s={this.Size}&");
            if (this.Rating != DEFAULT_RATING) sb.Append($"r={this.Rating.ToString().ToLowerInvariant()}&");
            if (!string.IsNullOrWhiteSpace(this.DefaultImage)) sb.Append($"d={this.DefaultImage}&");
            if (this.ForceDefault) sb.Append("f=y&");

            var url = sb.ToString().TrimEnd('?', '&');
            return url;
        }

        private string GetEmailHash() {
            var email = this.Email.Trim().ToLowerInvariant();
            var emailBytes = Encoding.ASCII.GetBytes(email);
            using (var md5 = MD5.Create()) {
                var hashBytes = md5.ComputeHash(emailBytes);
                var hashString = string.Join(string.Empty, hashBytes.Select(b => b.ToString("x2")));
                return hashString;
            }
        }

    }

    public static class GravatarDefaultImage {

        public const string Blank = "blank";
        public const string IdentIcon = "identicon";
        public const string MonsterId = "monsterid";
        public const string MysteryMan = "mm";
        public const string NotFound = "404";
        public const string Retro = "retro";
        public const string RoboHash = "robohash";
        public const string Wavatar = "wavatar";

    }

    public enum GravatarRating {
        G = 0,
        PG = 1,
        R = 2,
        X = 3
    }
}
