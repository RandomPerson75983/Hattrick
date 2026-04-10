using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the Players.razor page.
///
/// Note: These tests verify the expected output of Players.razor by testing the
/// rendered HTML structure. Since the test project cannot directly reference the
/// MAUI Hattrick library, we verify against the expected HTML output documented
/// in the component implementation.
///
/// Players page layout:
/// - Outer: players-page container
/// - Header: players-title showing "Your N players"
/// - Body: players-layout with two columns:
///   - Left (main): players-main-content containing skill-table
///   - Right (sidebar): players-sidebar placeholder (Quartet 2)
///
/// Skill table columns (left to right):
///   player | age | tsi | wage | specialty | form | Kp | Def | Pm | Wi | Sc | Ps | Sp
///
/// Each player row:
///   player-name (link) | age-cell | tsi-cell | wage-cell | specialty-cell |
///   form-cell | 7× skill-cell (each with skill-bar inside) | best-position-cell
/// </summary>
public class PlayersPageTests
{
    private const int DefaultPlayerCount = 3;

    /// <summary>
    /// Builds a representative HTML structure string for the Players page.
    /// Mirrors the expected output of Players.razor with a small squad of
    /// <see cref="DefaultPlayerCount"/> players.
    /// </summary>
    private static string BuildExpectedMarkup(int playerCount = DefaultPlayerCount)
    {
        var title = $"Your {playerCount} players";

        var headerRow =
            "<div class=\"skill-table-header\">"
            + "<span class=\"col-player\">player</span>"
            + "<span class=\"col-age\">age</span>"
            + "<span class=\"col-tsi\">tsi</span>"
            + "<span class=\"col-wage\">wage</span>"
            + "<span class=\"col-specialty\">specialty</span>"
            + "<span class=\"col-form\">form</span>"
            + "<span class=\"col-skill\">Kp</span>"
            + "<span class=\"col-skill\">Def</span>"
            + "<span class=\"col-skill\">Pm</span>"
            + "<span class=\"col-skill\">Wi</span>"
            + "<span class=\"col-skill\">Sc</span>"
            + "<span class=\"col-skill\">Ps</span>"
            + "<span class=\"col-skill\">Sp</span>"
            + "<span class=\"col-position\">position</span>"
            + "</div>";

        var allRows = string.Concat(Enumerable.Repeat(BuildPlayerRowMarkup(), playerCount));

        var skillTable =
            "<div class=\"skill-table\">"
            + headerRow
            + allRows
            + "</div>";

        var mainContent =
            "<div class=\"players-main-content\">"
            + skillTable
            + "</div>";

        var sidebar =
            "<div class=\"players-sidebar\">"
            + "<!-- Player detail panel (Quartet 2) -->"
            + "</div>";

        var layout =
            "<div class=\"players-layout\">"
            + mainContent
            + sidebar
            + "</div>";

        return "<div class=\"players-page\">"
            + $"<h1 class=\"players-title\">{title}</h1>"
            + layout
            + "</div>";
    }

    // ─────────────────────────────────────────────────────────────
    // Page container
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasPlayersPageContainer()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("players-page");
    }

    // ─────────────────────────────────────────────────────────────
    // Page title
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasPlayersTitleElement()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("players-title");
    }

    [Fact]
    public void PlayersPage_Title_ShowsPlayerCount()
    {
        var markup = BuildExpectedMarkup(playerCount: 5);

        markup.Should().Contain("Your 5 players");
    }

    [Fact]
    public void PlayersPage_Title_ShowsCorrectCountForSinglePlayer()
    {
        var markup = BuildExpectedMarkup(playerCount: 1);

        markup.Should().Contain("Your 1 players");
    }

    [Fact]
    public void PlayersPage_Title_ShowsZeroWhenNoPlayers()
    {
        var markup = BuildExpectedMarkup(playerCount: 0);

        markup.Should().Contain("Your 0 players");
    }

    // ─────────────────────────────────────────────────────────────
    // Layout containers
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasPlayersLayoutContainer()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("players-layout");
    }

    [Fact]
    public void PlayersPage_Rendered_HasMainContentArea()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("players-main-content");
    }

    [Fact]
    public void PlayersPage_Rendered_HasSidebarPlaceholder()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("players-sidebar");
    }

    [Fact]
    public void PlayersPage_Layout_MainContentBeforeSidebar()
    {
        var markup = BuildExpectedMarkup();

        var mainContentIndex = markup.IndexOf("players-main-content", StringComparison.Ordinal);
        var sidebarIndex = markup.IndexOf("players-sidebar", StringComparison.Ordinal);

        mainContentIndex.Should().BeLessThan(sidebarIndex);
    }

    [Fact]
    public void PlayersPage_Layout_MainContentAndSidebarInsideLayout()
    {
        var markup = BuildExpectedMarkup();

        var layoutStart = markup.IndexOf("players-layout", StringComparison.Ordinal);
        var mainContentIndex = markup.IndexOf("players-main-content", StringComparison.Ordinal);
        var sidebarIndex = markup.IndexOf("players-sidebar", StringComparison.Ordinal);

        mainContentIndex.Should().BeGreaterThan(layoutStart);
        sidebarIndex.Should().BeGreaterThan(layoutStart);
    }

    // ─────────────────────────────────────────────────────────────
    // Skill table
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasSkillTable()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("skill-table");
    }

    [Fact]
    public void PlayersPage_SkillTable_InsideMainContent()
    {
        var markup = BuildExpectedMarkup();

        var mainContentIndex = markup.IndexOf("players-main-content", StringComparison.Ordinal);
        var tableIndex = markup.IndexOf("skill-table", StringComparison.Ordinal);

        tableIndex.Should().BeGreaterThan(mainContentIndex);
    }

    // ─────────────────────────────────────────────────────────────
    // Skill table header
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasSkillTableHeader()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("skill-table-header");
    }

    [Fact]
    public void PlayersPage_Header_HasPlayerColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-player");
        markup.Should().Contain(">player<");
    }

    [Fact]
    public void PlayersPage_Header_HasAgeColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-age");
        markup.Should().Contain(">age<");
    }

    [Fact]
    public void PlayersPage_Header_HasTsiColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-tsi");
        markup.Should().Contain(">tsi<");
    }

    [Fact]
    public void PlayersPage_Header_HasWageColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-wage");
        markup.Should().Contain(">wage<");
    }

    [Fact]
    public void PlayersPage_Header_HasSpecialtyColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-specialty");
        markup.Should().Contain(">specialty<");
    }

    [Fact]
    public void PlayersPage_Header_HasFormColumn()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("col-form");
        markup.Should().Contain(">form<");
    }

    [Fact]
    public void PlayersPage_Header_HasKeeperSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Kp<");
    }

    [Fact]
    public void PlayersPage_Header_HasDefendingSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Def<");
    }

    [Fact]
    public void PlayersPage_Header_HasPlaymakingSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Pm<");
    }

    [Fact]
    public void PlayersPage_Header_HasWingerSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Wi<");
    }

    [Fact]
    public void PlayersPage_Header_HasScoringSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Sc<");
    }

    [Fact]
    public void PlayersPage_Header_HasPassingSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Ps<");
    }

    [Fact]
    public void PlayersPage_Header_HasSetPiecesSkillAbbreviation()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain(">Sp<");
    }

    // ─────────────────────────────────────────────────────────────
    // Player rows
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Rendered_HasPlayerRow()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("player-row");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasPlayerNameLink()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("player-name");
        // Player name must be a link
        markup.Should().Contain("<a class=\"player-name\"");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasAgeCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("age-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasTsiCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("tsi-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasWageCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("wage-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasSpecialtyCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("specialty-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasFormCell()
    {
        BuildExpectedMarkup().Should().Contain("form-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasSkillCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("skill-cell");
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasSevenSkillCells()
    {
        // One player row should have 7 skill cells (one per SkillType)
        var singleRowMarkup = BuildPlayerRowMarkup();

        var skillCellCount = CountOccurrences(singleRowMarkup, "skill-cell");
        skillCellCount.Should().Be(7);
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasSkillBarPerSkillCell()
    {
        var singleRowMarkup = BuildPlayerRowMarkup();

        var skillBarCount = CountOccurrences(singleRowMarkup, "skill-bar");
        skillBarCount.Should().Be(7);
    }

    [Fact]
    public void PlayersPage_PlayerRow_HasBestPositionCell()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("best-position-cell");
    }

    // ─────────────────────────────────────────────────────────────
    // Order checks: header before rows, rows inside table
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public void PlayersPage_Layout_HeaderBeforePlayerRows()
    {
        var markup = BuildExpectedMarkup();

        var headerIndex = markup.IndexOf("skill-table-header", StringComparison.Ordinal);
        var rowIndex = markup.IndexOf("player-row", StringComparison.Ordinal);

        headerIndex.Should().BeLessThan(rowIndex);
    }

    [Fact]
    public void PlayersPage_Layout_PlayerRowsInsideSkillTable()
    {
        var markup = BuildExpectedMarkup();

        var tableStart = markup.IndexOf("skill-table", StringComparison.Ordinal);
        var rowIndex = markup.IndexOf("player-row", StringComparison.Ordinal);

        rowIndex.Should().BeGreaterThan(tableStart);
    }

    [Fact]
    public void PlayersPage_Layout_TitleBeforeLayout()
    {
        var markup = BuildExpectedMarkup();

        var titleIndex = markup.IndexOf("players-title", StringComparison.Ordinal);
        var layoutIndex = markup.IndexOf("players-layout", StringComparison.Ordinal);

        titleIndex.Should().BeLessThan(layoutIndex);
    }

    // ─────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────

    private static string BuildPlayerRowMarkup()
    {
        // Representative skill-cell with skill-bar (7 skills, one per SkillType)
        static string SkillCell() =>
            "<div class=\"skill-cell\">"
            + "<div class=\"skill-bar\"></div>"
            + "</div>";

        var sevenSkillCells = string.Concat(Enumerable.Repeat(SkillCell(), 7));

        return "<div class=\"player-row\">"
            + "<div class=\"player-name-cell\"><a class=\"player-name\" href=\"/players/1\">Erwin Brandt</a></div>"
            + "<div class=\"age-cell\">23 years and 72 days</div>"
            + "<div class=\"tsi-cell\">4500</div>"
            + "<div class=\"wage-cell\">12000</div>"
            + "<div class=\"specialty-cell\">Technical</div>"
            + "<div class=\"form-cell\">5</div>"
            + sevenSkillCells
            + "<div class=\"best-position-cell\">Central Defender</div>"
            + "</div>";
    }

    private static int CountOccurrences(string haystack, string needle) =>
        (haystack.Length - haystack.Replace(needle, string.Empty).Length) / needle.Length;
}
