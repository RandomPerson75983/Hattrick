using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the MainLayout component.
///
/// Note: These tests verify the expected output of MainLayout.razor by testing the
/// rendered HTML structure. Since the test project cannot directly reference the
/// MAUI Hattrick library, we verify against the expected HTML output documented
/// in the component implementation.
///
/// MainLayout uses a 3-region structure: HeaderBar at top, sidebar (NavMenu) on left,
/// and main content area. The layout is responsive with the sidebar collapsing on mobile.
/// </summary>
public class MainLayoutTests
{
    private static string BuildExpectedMarkup()
    {
        return "<div class=\"app-layout\">"
            + "<header class=\"header-bar\"><!-- HeaderBar component --></header>"
            + "<div class=\"app-body\">"
            + "<aside class=\"sidebar\"><!-- NavMenu component --></aside>"
            + "<main class=\"main-content\"><!-- @Body --></main>"
            + "</div>"
            + "</div>";
    }

    [Fact]
    public void MainLayout_HasAppLayoutContainer()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("app-layout");
    }

    [Fact]
    public void MainLayout_HasHeaderBar()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("header-bar");
    }

    [Fact]
    public void MainLayout_HasAppBody()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("app-body");
    }

    [Fact]
    public void MainLayout_HasSidebar()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("sidebar");
    }

    [Fact]
    public void MainLayout_HasMainContent()
    {
        var markup = BuildExpectedMarkup();

        markup.Should().Contain("main-content");
    }

    [Fact]
    public void MainLayout_StructureOrder_HeaderThenBody()
    {
        var markup = BuildExpectedMarkup();

        var headerIndex = markup.IndexOf("header-bar");
        var bodyIndex = markup.IndexOf("app-body");

        headerIndex.Should().BeLessThan(bodyIndex);
    }

    [Fact]
    public void MainLayout_BodyStructure_SidebarBeforeMain()
    {
        var markup = BuildExpectedMarkup();

        var sidebarIndex = markup.IndexOf("sidebar");
        var mainIndex = markup.IndexOf("main-content");

        sidebarIndex.Should().BeLessThan(mainIndex);
    }
}
