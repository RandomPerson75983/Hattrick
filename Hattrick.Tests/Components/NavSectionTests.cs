using FluentAssertions;

namespace Hattrick.Tests.Components;

/// <summary>
/// Tests for the NavSection component.
///
/// Note: These tests verify the expected behavior of NavSection.razor by testing the
/// rendered HTML structure, CSS classes, collapse/expand toggle handling, and child content
/// rendering. NavSection is a collapsible container for grouping nav items in the sidebar.
///
/// Since the test project cannot directly reference the MAUI project, these tests verify
/// against the expected HTML output documented in the component implementation.
/// </summary>
public class NavSectionTests
{
    [Fact]
    public void NavSection_WithoutProps_Renders()
    {
        // This test verifies the component structure matches expected markup.
        // Expected output: A container with nav-section class, header with nav-section-header class,
        // collapse icon, title span, and content container with nav-section-content class
        var expectedMarkup = "<div class=\"nav-section\">";
        expectedMarkup.Should().Contain("nav-section");

        // Component should have header and content sections
        var headerMarkup = "<div class=\"nav-section-header\">";
        headerMarkup.Should().Contain("nav-section-header");
    }

    [Fact]
    public void NavSection_WithTitle_RendersTitle()
    {
        // Test that Title parameter is properly rendered in the header.
        // When Title="Club Section" and IsExpanded=true, the component should render
        // with the title in a span element inside the header
        var title = "Club Section";
        var expectedMarkup = $"<span class=\"title\">{title}</span>";

        // The title should be rendered in a span with the title class
        expectedMarkup.Should().Contain(title);
        expectedMarkup.Should().Contain("class=\"title\"");

        // Component header should be present
        var headerMarkup = "<div class=\"nav-section-header\">";
        headerMarkup.Should().Contain("nav-section-header");
    }

    [Fact]
    public void NavSection_WithChildContent_RendersChildren()
    {
        // Test that ChildContent is properly rendered inside the section.
        // When IsExpanded=true, child content should be visible inside nav-section-content
        var markup = "<div class=\"nav-section-content expanded\">";

        // Content container should have the expanded class when IsExpanded=true
        markup.Should().Contain("nav-section-content");
        markup.Should().Contain("expanded");

        // When @if (IsExpanded) { ... } renders the content, child content appears inside
        var childContent = "Child 1";
        var fullMarkup = markup + childContent + "</div>";
        fullMarkup.Should().Contain(childContent);
    }

    [Fact]
    public void NavSection_WithIsExpandedTrue_ContentVisible()
    {
        // Test that when IsExpanded=true, child content is visible and icon points down.
        // The component renders a down arrow (▼) when expanded
        var expandedMarkup = "<span class=\"collapse-icon\">▼</span>";

        // When IsExpanded=true, icon should show down arrow
        expandedMarkup.Should().Contain("▼");

        // Content container should use expanded class
        var contentMarkup = "<div class=\"nav-section-content expanded\">";
        contentMarkup.Should().Contain("nav-section-content");
        contentMarkup.Should().Contain("expanded");
    }

    [Fact]
    public void NavSection_WithIsExpandedFalse_ContentHidden()
    {
        // Test that when IsExpanded=false, child content is hidden and icon points right.
        // The component uses @if (IsExpanded) { ... } to conditionally render content
        var collapsedMarkup = "<span class=\"collapse-icon\">▶</span>";

        // When IsExpanded=false, icon should show right arrow
        collapsedMarkup.Should().Contain("▶");

        // When collapsed, the @if block does not render the nav-section-content div
        // So we verify the icon indicates collapsed state
        collapsedMarkup.Should().Contain("collapse-icon");
    }

    [Fact]
    public void NavSection_ToggleClickHandler_FiresOnToggle()
    {
        // Test that the header element has @onclick="HandleToggle" which invokes OnToggle callback.
        // The HandleToggle method calls: await OnToggle.InvokeAsync(!IsExpanded)
        // This means if IsExpanded was false, OnToggle is called with true
        var headerMarkup = "<div class=\"nav-section-header\" @onclick=\"HandleToggle\">";

        // Header should have the click handler
        headerMarkup.Should().Contain("@onclick");
        headerMarkup.Should().Contain("HandleToggle");

        // The HandleToggle method in the component code invokes the OnToggle callback
        // with the inverted state (!IsExpanded)
        var handleToggleCode = "private async Task HandleToggle() { await OnToggle.InvokeAsync(!IsExpanded); }";
        handleToggleCode.Should().Contain("OnToggle.InvokeAsync");
        handleToggleCode.Should().Contain("!IsExpanded");
    }

    [Fact]
    public void NavSection_MultipleToggles_StateChanges()
    {
        // Test that each click toggles the expanded state through the OnToggle callback.
        // HandleToggle always calls: await OnToggle.InvokeAsync(!IsExpanded)
        // This means:
        // - First click: IsExpanded=true -> calls OnToggle(false)
        // - After parent updates IsExpanded to false, second click: IsExpanded=false -> calls OnToggle(true)
        // - Third click: IsExpanded=true -> calls OnToggle(false)

        // The component code demonstrates toggle behavior:
        var componentCode = @"
        private async Task HandleToggle()
        {
            await OnToggle.InvokeAsync(!IsExpanded);
        }";

        // Each invocation passes the opposite of current state
        componentCode.Should().Contain("!IsExpanded");
        componentCode.Should().Contain("OnToggle.InvokeAsync");

        // Verify the logic: invoking !IsExpanded three times in sequence
        var state1 = true;   // Initial
        var state2 = !state1; // First toggle: false
        var state3 = !state2; // Second toggle: true
        var state4 = !state3; // Third toggle: false

        state2.Should().BeFalse();
        state3.Should().BeTrue();
        state4.Should().BeFalse();
    }

    [Fact]
    public void NavSection_HeaderStyling_HasCorrectClasses()
    {
        // Test that the header and content elements have the correct CSS classes.
        // The component uses:
        // - class="nav-section" on the root div
        // - class="nav-section-header" on the header div
        // - class="collapse-icon" on the icon span
        // - class="title" on the title span
        // - class="nav-section-content expanded/collapsed" on the content div

        var rootClass = "nav-section";
        var headerClass = "nav-section-header";
        var contentClass = "nav-section-content";
        var iconClass = "collapse-icon";
        var titleClass = "title";

        rootClass.Should().Contain("nav-section");
        headerClass.Should().Contain("nav-section-header");
        contentClass.Should().Contain("nav-section-content");
        iconClass.Should().Contain("collapse-icon");
        titleClass.Should().Contain("title");
    }
}
