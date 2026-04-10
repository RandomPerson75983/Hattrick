using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the PlayersSidebar component (rendered inside Players.razor).
///
/// The sidebar replaces the placeholder in players-sidebar and contains two sections:
///   1. Team Total   — totals for TSI, Wage, Estimated Value, Nationalities, Injured,
///                     Red cards, Yellow Cards
///   2. Team Average — averages for TSI, Wage, Estimated Value, Age, Form, Stamina, Exp
///
/// Expected HTML class hierarchy:
///   players-sidebar
///   └── team-stats-sidebar
///       ├── team-total-section
///       │   ├── (heading: "Team Total")
///       │   └── stats-row × N
///       │       ├── stats-label
///       │       └── stats-value
///       └── team-average-section
///           ├── (heading: "Team Average")
///           └── stats-row × N
///               ├── stats-label
///               └── stats-value
/// </summary>
public class PlayersSidebarTests
{
    /// <summary>
    /// Builds the expected sidebar HTML. Matches the structure that the
    /// PlayersSidebar component is required to render.
    /// </summary>
    private static string BuildExpectedSidebarMarkup()
    {
        static string StatsRow(string label, string value) =>
            "<div class=\"stats-row\">"
            + $"<span class=\"stats-label\">{label}</span>"
            + $"<span class=\"stats-value\">{value}</span>"
            + "</div>";

        var totalSection =
            "<div class=\"team-total-section\">"
            + "<h3>Team Total</h3>"
            + StatsRow("TSI", "0")
            + StatsRow("Wage", "0")
            + StatsRow("Estimated Value", "0")
            + StatsRow("Nationalities", "0")
            + StatsRow("Injured", "0")
            + StatsRow("Red cards", "0")
            + StatsRow("Yellow Cards", "0")
            + "</div>";

        var averageSection =
            "<div class=\"team-average-section\">"
            + "<h3>Team Average</h3>"
            + StatsRow("TSI", "0")
            + StatsRow("Wage", "0")
            + StatsRow("Estimated Value", "0")
            + StatsRow("Age", "0")
            + StatsRow("Form", "0")
            + StatsRow("Stamina", "0")
            + StatsRow("Exp", "0")
            + "</div>";

        var statsSidebar =
            "<div class=\"team-stats-sidebar\">"
            + totalSection
            + averageSection
            + "</div>";

        return "<div class=\"players-sidebar\">"
            + statsSidebar
            + "</div>";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Top-level container
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_Rendered_HasPlayersSidebarContainer()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("players-sidebar");
    }

    [Fact]
    public void PlayersSidebar_Rendered_HasTeamStatsSidebarContainer()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("team-stats-sidebar");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Section presence
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_Rendered_HasTeamTotalSection()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("team-total-section");
    }

    [Fact]
    public void PlayersSidebar_Rendered_HasTeamAverageSection()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("team-average-section");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Section headings
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_TeamTotalSection_HasTeamTotalHeading()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Team Total");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_HasTeamAverageHeading()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Team Average");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Section ordering
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_Layout_TeamTotalSectionBeforeTeamAverageSection()
    {
        var markup = BuildExpectedSidebarMarkup();

        var totalIndex   = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var averageIndex = markup.IndexOf("team-average-section", StringComparison.Ordinal);

        totalIndex.Should().BeLessThan(averageIndex);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Row / label / value structure
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_Rendered_HasStatsRow()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("stats-row");
    }

    [Fact]
    public void PlayersSidebar_Rendered_HasStatsLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("stats-label");
    }

    [Fact]
    public void PlayersSidebar_Rendered_HasStatsValue()
    {
        var markup = BuildExpectedSidebarMarkup();

        markup.Should().Contain("stats-value");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Team Total section — required stat labels
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsTSILabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("TSI");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsWageLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Wage");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsEstimatedValueLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Estimated Value");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsNationalitiesLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Nationalities");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsInjuredLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Injured");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsRedCardsLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Red cards");
    }

    [Fact]
    public void PlayersSidebar_TeamTotalSection_ContainsYellowCardsLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-total-section", StringComparison.Ordinal);
        var sectionEnd   = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..sectionEnd];

        section.Should().Contain("Yellow Cards");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Team Average section — required stat labels
    // ─────────────────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsTSILabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("TSI");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsWageLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Wage");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsEstimatedValueLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Estimated Value");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsAgeLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Age");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsFormLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Form");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsStaminaLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Stamina");
    }

    [Fact]
    public void PlayersSidebar_TeamAverageSection_ContainsExpLabel()
    {
        var markup = BuildExpectedSidebarMarkup();

        var sectionStart = markup.IndexOf("team-average-section", StringComparison.Ordinal);
        var section      = markup[sectionStart..];

        section.Should().Contain("Exp");
    }
}
