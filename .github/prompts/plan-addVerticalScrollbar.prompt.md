# Plan: Add Vertical Scrollbar to BenchView and RecentPage

Establish viewport height containment and add scrollable containers to both the BenchView component and RecentPage list, preventing the browser viewport from scrolling when content overflows.

## Steps

1. **Establish viewport containment in [MainLayout.razor.css](src/WpfBlazor/Layout/MainLayout.razor.css)** — Add `height: 100vh` to `.page` class and `min-height: 0` to `main` to create a fixed-height flex container that fills the viewport.

2. **Add global height chain in [app.css](src/WpfBlazor/wwwroot/css/app.css)** — Set `height: 100%` on `html`, `body`, and `#app` elements, plus `overflow: hidden` on body to prevent document-level scrolling.

3. **Convert BenchPage to flex layout in [BenchPage.razor.css](src/WpfBlazor/Pages/BenchPage.razor.css)** — Make `.container` a flex column with `height: 100%`, and add new `.bench-view-container` class with `flex: 1`, `min-height: 0`, `overflow-y: auto`, `border: 1px solid #ccc`, and `padding: 10px`.

4. **Wrap BenchView in scroll container in [BenchPage.razor](src/WpfBlazor/Pages/BenchPage.razor)** — Add a `<div class="bench-view-container">` wrapper around the `<BenchView>` component to contain the scrollable area.

5. **Convert RecentPage to flex layout in [RecentPage.razor.css](src/WpfBlazor/Pages/RecentPage.razor.css)** — Add flex column layout to page root, and create new `.recent-list-container` class with `flex: 1`, `min-height: 0`, `overflow-y: auto`, `border: 1px solid #ccc`, and `padding: 10px` for the `<ul>` wrapper.

6. **Wrap RecentBenchView list in scroll container in [RecentPage.razor](src/WpfBlazor/Pages/RecentPage.razor)** — Add a `<div class="recent-list-container">` wrapper around the existing `<ul>` containing the `@foreach` loop of `<RecentBenchView>` components.

## Further Considerations

1. **Alternative simpler approach** — Instead of full layout restructuring, add a `max-height: 60vh` wrapper directly in [BenchView.razor.css](src/WpfBlazor/Components/BenchView.razor.css) and [BenchView.razor](src/WpfBlazor/Components/BenchView.razor), and similarly for RecentPage. Trade-off: Less precise height calculation but requires fewer file changes and avoids global layout modifications.

2. **Consistent scroll container heights** — Both BenchPage and RecentPage will have different fixed header content (buttons/separator vs h3/status), so their scroll containers will have slightly different available heights despite using the same `flex: 1` approach. This is the correct and expected behavior - each page uses all available space after accounting for its specific fixed content.
