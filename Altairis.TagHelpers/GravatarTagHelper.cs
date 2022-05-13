using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers;

public class GravatarTagHelper : TagHelper {
    private const int DEFAULT_SIZE = 80;
    private const GravatarRating DEFAULT_RATING = GravatarRating.G;
    private readonly GravatarOptions options;

    public GravatarTagHelper(GravatarOptions options = null) {
        this.options = options ?? GravatarOptions.Default;
    }

    public string Email { get; set; }

    public int? Size { get; set; }

    private int EffectiveSize => this.Size ?? (this.options.Size ?? GravatarOptions.DefaultSize);

    public string DefaultImage { get; set; }

    private string EffectiveDefaultImage => this.DefaultImage ?? this.options.DefaultImage;

    public GravatarRating? Rating { get; set; }

    private GravatarRating EffectiveRating => this.Rating ?? (this.options.Rating ?? GravatarOptions.DefaultRating);

    public bool? ForceDefault { get; set; }

    private bool EffectiveForceDefault => this.ForceDefault ?? (this.options?.ForceDefault ?? false);

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        base.Process(context, output);

        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;

        output.Attributes.Add("src", this.GetGravatarUrl());
        output.Attributes.Add("width", this.EffectiveSize);
        output.Attributes.Add("height", this.EffectiveSize);

        if (context.AllAttributes["alt"] == null) output.Attributes.Add("alt", "Gravatar");
    }

    private string GetGravatarUrl() {
        // Construct base URL
        var sb = new StringBuilder();
        sb.Append($"https://www.gravatar.com/avatar/{this.GetEmailHash()}?");

        // Add optional parameters
        if (this.EffectiveSize != DEFAULT_SIZE) sb.Append($"s={this.EffectiveSize}&");
        if (this.EffectiveRating != DEFAULT_RATING) sb.Append($"r={this.EffectiveRating.ToString().ToLowerInvariant()}&");
        if (!string.IsNullOrWhiteSpace(this.EffectiveDefaultImage)) sb.Append($"d={this.EffectiveDefaultImage}&");
        if (this.EffectiveForceDefault) sb.Append("f=y&");

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

public class GravatarOptions {
    public const int DefaultSize = 80;
    public const GravatarRating DefaultRating = GravatarRating.G;

    public int? Size { get; set; }

    public string DefaultImage { get; set; }

    public GravatarRating? Rating { get; set; }

    public bool ForceDefault { get; set; }

    public static readonly GravatarOptions Default = new() {
        Size = DefaultSize,
        Rating = DefaultRating
    };

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
