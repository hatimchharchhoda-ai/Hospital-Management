# Matrix Design System Registry

A theme-first, curated registry for **Matrix** — built on the [shadcn registry](https://ui.shadcn.com/docs/registry) system and published under the `@matrix` namespace.

---

## What is this?

This registry is the single source of truth for UI components, themes, and fonts used across Matrix products. All component source files live **inside this repository** and are served directly to consumer projects — nothing is fetched from upstream at install time.

Behind the scenes, items fall into two categories:

| Type | What it means |
|---|---|
| **Matrix-owned** | Brand-specific source: `trust-blue` theme, `font-geist`, `font-geist-mono`, and any custom Matrix components. |
| **Curated shadcn components** | shadcn primitives (button, badge, avatar, …) vendored into `components/ui/` and re-served from this registry, themed automatically by Trust Blue tokens. `meta.links` records the upstream source for traceability. |

Consumers interact with **one namespace only: `@matrix`**. Whether a component originated from shadcn or was authored in-house makes no difference — the install command is identical.

---

## Repository structure

```
matrix-registry/
├── components/
│   └── ui/                  # Vendored shadcn primitives + Matrix custom components
│       ├── button.tsx
│       ├── badge.tsx
│       └── ...
├── registry/
│       └── blocks/          # Matrix-authored blocks and compositions
├── registry.json            # Source of truth — all registry item definitions
├── public/
│   └── r/                   # ⚠ Generated — do not edit manually
│       ├── registry.json
│       ├── button.json
│       └── ...
└── components.json
```

> `public/r/` is built by `pnpm registry:build` and regenerated on every `pnpm dev`. Never edit files in `public/r/` directly.

---

## For Consumers

### Prerequisites

- Node.js 18+
- `pnpm`
- A React project with [shadcn already initialised](https://ui.shadcn.com/docs/installation) — a `components.json` must exist in the project root

### Step 1 — Register the `@matrix` namespace

Open your project's `components.json` and add the `registries` field pointing to the running registry:

```json
{
  "$schema": "https://ui.shadcn.com/schema.json",
  "style": "radix-nova",
  "rsc": false,
  "tsx": true,
  "tailwind": {
    "config": "",
    "css": "src/index.css",
    "baseColor": "neutral",
    "cssVariables": true,
    "prefix": ""
  },
  "iconLibrary": "lucide",
  "aliases": {
    "components": "@/components",
    "utils": "@/lib/utils",
    "ui": "@/components/ui",
    "lib": "@/lib",
    "hooks": "@/hooks"
  },
  "registries": {
    "@matrix": "http://localhost:3001/r/{name}.json"
  }
}
```

**Two fields matter most:**

- `"css"` under `tailwind` — tells the shadcn CLI where to inject CSS variables (theme tokens, font faces). `src/index.css` is the default for Vite projects. Change it if your entry CSS lives elsewhere (`app/globals.css`, `styles/main.css`, etc.).
- `"registries"` — maps the `@matrix` shorthand to the registry URL. The `{name}` placeholder is replaced by the CLI with the component name at install time.

### Step 2 — Start the registry server

The registry must be running before you install anything. From the **registry project root**:

```bash
pnpm dev -p 3001
```

This builds `public/r/` and serves the registry at `http://localhost:3001`. Keep this terminal open while running `shadcn add` commands in your consumer project.

### Step 3 — Install the theme and fonts first

The `trust-blue` theme injects all CSS custom properties that every component relies on. Install it before anything else, or components will render without the correct colour tokens.

```bash
pnpm dlx shadcn@latest add @matrix/trust-blue @matrix/font-geist @matrix/font-geist-mono
```

### Step 4 — Install components

With the theme in place, install any component or block:

```bash
# A single primitive
pnpm dlx shadcn@latest add @matrix/button

# Multiple at once
pnpm dlx shadcn@latest add @matrix/button @matrix/badge @matrix/avatar

# A full block
pnpm dlx shadcn@latest add @matrix/dashboard-01
```

Each command fetches the component's `.tsx` source from the registry and writes it into the path defined by your `aliases` in `components.json` (typically `src/components/ui/`).

### Dry-run before committing

Unsure what a command will write? Use `--dry-run` to preview the file changes without touching your project:

```bash
pnpm dlx shadcn@latest add @matrix/dashboard-01 --dry-run
```

### Listing all available items

```bash
pnpm exec shadcn list http://localhost:3001/r/registry.json
```

---

## How installation works (under the hood)

```
Consumer project                     Registry (localhost:3001)
────────────────                     ─────────────────────────
components.json
  └─ @matrix ──────────────────────► GET /r/{name}.json
                                           │
                                           │  descriptor contains:
                                           │  • files[]  ← component source paths
                                           │  • dependencies[]  ← npm packages
                                           │  • meta.links  ← traceability only
                                           │
                                     GET /r/{file-path}
                                           │
                                           ▼
                                     component .tsx written into
                                     consumer's src/components/ui/
```

1. The CLI resolves `@matrix/button` → `http://localhost:3001/r/button.json`.
2. It reads the descriptor: which files to fetch, which npm packages to install.
3. Each file listed in `files[]` is downloaded **from this registry** — not from any upstream — and written to the consumer project.
4. npm dependencies (e.g. `@radix-ui/react-slot`) are added to `package.json` and installed automatically.
5. Because `trust-blue` is already in the consumer's CSS, every component is themed on arrival.

`meta.links.source` in the descriptor is documentation only — it records where a vendored component originally came from. The CLI does not follow it.

---

## For Maintainers

### Running the registry

```bash
pnpm dev -p 3001
```

The `predev` lifecycle hook runs `pnpm registry:build` automatically. To build without starting the server:

```bash
pnpm registry:build
```

### How `registry.json` drives everything

Every item served by the registry is declared in `registry.json`. The build step transforms this into individual `public/r/{name}.json` files that the shadcn CLI understands.

A typical curated shadcn component entry looks like this:

```json
{
  "name": "button",
  "type": "registry:ui",
  "title": "Button",
  "description": "Displays a button or a component that looks like a button. Curated Matrix alias for @shadcn/button, themed via trust-blue tokens.",
  "dependencies": ["@radix-ui/react-slot"],
  "files": [
    {
      "path": "components/ui/button.tsx",
      "type": "registry:ui",
      "target": ""
    }
  ],
  "meta": {
    "links": {
      "source": "https://xyz/button.json",
      "docs": "https://xyz/button",
      "homepage": "https://xyz.com"
    }
  }
}
```

Key fields:

| Field | Purpose |
|---|---|
| `name` | The identifier used in `@matrix/<name>` install commands |
| `type` | `registry:ui` for primitives, `registry:block` for compositions, `registry:theme` for themes |
| `dependencies` | npm packages the component needs — added to the consumer's `package.json` at install time |
| `files[].path` | Path to the source file **inside this repository** |
| `files[].target` | Where the file is written in the consumer project (empty string = use the CLI default from `components.json` aliases) |
| `meta.links` | Traceability only — the shadcn CLI does not read this field |

### Adding a vendored shadcn primitive

1. Copy the component source from the shadcn upstream into `components/ui/`.
2. Add an entry to `registry.json` with `files` pointing to that local file and `meta.links.source` referencing the upstream commit/tag URL for traceability.
3. List any Radix or other npm dependencies in `dependencies`.

```json
{
  "name": "alert-dialog",
  "type": "registry:ui",
  "title": "Alert Dialog",
  "description": "A modal dialog that interrupts the user with important content and expects a response. Curated Matrix alias for @shadcn/alert-dialog, themed via trust-blue tokens.",
  "files": [
    {
      "path": "components/ui/alert-dialog.tsx",
      "type": "registry:ui",
      "target": ""
    }
  ],
  "meta": {
    "links": {
      "source": "https://raw.githubusercontent.com/shadcn-ui/ui/shadcn@4.16.0/apps/v4/public/r/styles/new-york/alert-dialog.json",
      "docs": "https://ui.shadcn.com/docs/components/alert-dialog",
      "homepage": "https://ui.shadcn.com"
    }
  }
}
```

### Adding a Matrix custom component

Author the source under `registry/` (or `components/ui/` if it's a primitive-level component) and add an entry to `registry.json`. Custom components can depend on other `@matrix` items or on shadcn primitives already in the registry.

```json
{
  "name": "dashboard-shell",
  "type": "registry:block",
  "title": "Dashboard Shell",
  "description": "Matrix-branded dashboard layout with sidebar and header.",
  "registryDependencies": ["@matrix/sidebar", "@matrix/trust-blue"],
  "files": [
    {
      "path": "registry/new-york/blocks/dashboard-shell/page.tsx",
      "type": "registry:page",
      "target": "app/dashboard/page.tsx"
    }
  ]
}
```

### Adding a live preview

Previews are opt-in and only relevant for Matrix-authored components with local files.

1. Add the item and its `files` to `registry.json`.
2. Export a renderable demo from `components/previews/index.ts`, keyed by the registry item name:

```ts
export const previewNames = ["matrix-sidebar-07"] as const;
```

The catalog renders `app/preview/<name>` in an isolated iframe. The explicit lookup map is required because Next.js must know which React modules to compile at build time — `registry.json` alone is not enough.

### Verification checklist

Run these before every PR:

```bash
pnpm registry:build       # registry.json must compile without errors
pnpm exec tsc --noEmit    # TypeScript must be clean
pnpm lint                 # No lint violations
```

Verify the item installs correctly from a consumer project with `@matrix` configured:

```bash
pnpm dlx shadcn@latest add @matrix/<your-item> --dry-run
```

---

## Registry policy

| Rule | Detail |
|---|---|
| **Vendor, don't link** | All component source lives in this repository. Consumers never depend on external URLs at runtime. |
| **One namespace** | App teams install exclusively from `@matrix`. Direct installs from shadcn or other registries require design-system approval. |
| **Record provenance** | Every vendored shadcn file must have a `meta.links.source` URL pinned to the exact upstream commit or tag. |
| **Custom only when necessary** | If a shadcn primitive covers the need without modification, vendor it as-is. Author custom source only for genuine Matrix-specific requirements. |
| **Verify before merging** | `registry:build` + `tsc --noEmit` + `lint` must all pass. |

---

## Reference

- [`REGISTRY_MANAGEMENT.md`](./REGISTRY_MANAGEMENT.md) — full registry management guide
- [shadcn registry documentation](https://ui.shadcn.com/docs/registry) — schema and CLI reference
- [shadcn `components.json` reference](https://ui.shadcn.com/docs/components-json) — all `components.json` fields explained
- [shadcn registry directory](https://ui.shadcn.com/docs/directory) — upstream registry index (for sourcing new components to vendor)
