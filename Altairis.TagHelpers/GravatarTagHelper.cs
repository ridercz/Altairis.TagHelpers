using System.Security.Cryptography;
using System.Text;

namespace Altairis.TagHelpers;

public class GravatarTagHelper : TagHelper {
    private readonly GravatarOptions options;

    public GravatarTagHelper(IOptions<GravatarOptions> options) {
        this.options = options.Value ?? new();
    }

    public string Email { get; set; } = string.Empty;

    public int? Size { get; set; }

    public string? DefaultImage { get; set; }

    public GravatarRating? Rating { get; set; }

    public bool? ForceDefault { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        // Run parent code
        base.Process(context, output);

        // Update local values with options
        this.Size ??= this.options.Size;
        this.DefaultImage ??= this.options.DefaultImage;
        this.Rating ??= this.options.Rating;
        this.ForceDefault ??= this.options.ForceDefault;

        // Prepare output element
        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;
        output.Attributes.Add("src", this.GetGravatarUrl());
        output.Attributes.Add("width", this.Size);
        output.Attributes.Add("height", this.Size);
        if (context.AllAttributes["alt"] == null) output.Attributes.Add("alt", "Gravatar");
    }

    private string GetGravatarUrl() {
        // Construct base URL
        var sb = new StringBuilder();
        sb.Append($"https://www.gravatar.com/avatar/{this.GetEmailHash()}?");

        // Add optional parameters
        if (this.Size != GravatarOptions.DefaultSize) sb.Append($"s={this.Size}&");
        if (this.Rating != GravatarOptions.DefaultRating) sb.Append($"r={this.Rating.ToString().ToLowerInvariant()}&");
        if (!string.IsNullOrWhiteSpace(this.DefaultImage)) sb.Append($"d={this.DefaultImage}&");
        if (this.ForceDefault == true) sb.Append("f=y&");

        // Remove trailing character
        var url = sb.ToString().TrimEnd('?', '&');
        return url;
    }

    private string GetEmailHash() {
        var email = this.Email.Trim().ToLowerInvariant();
        var emailBytes = Encoding.ASCII.GetBytes(email);
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(emailBytes);
        var hashString = string.Join(string.Empty, hashBytes.Select(b => b.ToString("x2")));
        return hashString;
    }

}

public class GravatarOptions {
    public const int DefaultSize = 80;
    public const GravatarRating DefaultRating = GravatarRating.G;

    public int Size { get; set; } = DefaultSize;

    public string? DefaultImage { get; set; }

    public GravatarRating Rating { get; set; } = DefaultRating;

    public bool ForceDefault { get; set; } = false;

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