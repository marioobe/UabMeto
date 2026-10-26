---
name: Timor Heritage & Modern Learning
colors:
  surface: '#0b1519'
  surface-dim: '#0b1519'
  surface-bright: '#313a40'
  surface-container-lowest: '#070f14'
  surface-container-low: '#141d22'
  surface-container: '#182126'
  surface-container-high: '#222b31'
  surface-container-highest: '#2d363c'
  on-surface: '#dae4eb'
  on-surface-variant: '#d0c5b5'
  inverse-surface: '#dae4eb'
  inverse-on-surface: '#293237'
  outline: '#998f81'
  outline-variant: '#4d463a'
  surface-tint: '#e4c282'
  primary: '#ffefd7'
  on-primary: '#402d00'
  primary-container: '#f3d08f'
  on-primary-container: '#715823'
  inverse-primary: '#745b25'
  secondary: '#ffb599'
  on-secondary: '#5a1c00'
  secondary-container: '#7b3313'
  on-secondary-container: '#ffa17b'
  tertiary: '#e2f4ff'
  on-tertiary: '#1f333d'
  tertiary-container: '#c2d8e5'
  on-tertiary-container: '#4a5f69'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#ffdea3'
  primary-fixed-dim: '#e4c282'
  on-primary-fixed: '#261900'
  on-primary-fixed-variant: '#5a430f'
  secondary-fixed: '#ffdbce'
  secondary-fixed-dim: '#ffb599'
  on-secondary-fixed: '#370e00'
  on-secondary-fixed-variant: '#783111'
  tertiary-fixed: '#d0e6f3'
  tertiary-fixed-dim: '#b4cad6'
  on-tertiary-fixed: '#081e27'
  on-tertiary-fixed-variant: '#364954'
  background: '#0b1519'
  on-background: '#dae4eb'
  surface-variant: '#2d363c'
  surface-card: '#16242B'
  surface-border: '#223640'
  text-primary: '#F4EBDD'
  text-muted: '#B8AEA0'
  status-success: '#5FA97A'
  status-warning: '#E39B5C'
  status-danger: '#C6544C'
typography:
  headline-1:
    fontFamily: Plus Jakarta Sans
    fontSize: 24px
    fontWeight: '700'
    lineHeight: 32px
    letterSpacing: -0.01em
  headline-2:
    fontFamily: Plus Jakarta Sans
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
    letterSpacing: -0.005em
  headline-3:
    fontFamily: Plus Jakarta Sans
    fontSize: 16px
    fontWeight: '600'
    lineHeight: 24px
  body-regular:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-small:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-caps:
    fontFamily: Plus Jakarta Sans
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.06em
  label-chip:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
  interactive-button:
    fontFamily: Plus Jakarta Sans
    fontSize: 16px
    fontWeight: '700'
    lineHeight: 24px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  gutter: 1rem
  margin: 1rem
  space-xs: 0.25rem
  space-sm: 0.5rem
  space-md: 0.75rem
  space-lg: 1rem
  space-xl: 1.5rem
---

## Brand & Style

### Personality & Emotional Response
The design system articulates cultural reverence, intellectual calm, and focused modern learning. It departs from gamified neon aesthetics, instead adopting a grounded atmosphere rooted in Timorese visual identity. The experience evokes warmth, dignity, and reliability—reinforcing the message that endangered regional languages are modern, living systems of communication worthy of structured preservation.

### Visual Movement & Design Style
The system employs a **Flat Tonal Minimalism** methodology tailored for mobile efficiency. Visual clarity takes priority over computational overhead:
- **No intensive GPU operations:** Zero heavy drop shadows, blurred glass layers, or dynamic mesh gradients to ensure frictionless rendering on budget Android chipsets.
- **Surface Elevation by Tone:** Depth is communicated through calibrated surface luminance steps rather than lighting shaders.
- **Cultural Motif Discipline:** Abstracted Timor ikat geometric accents (sharp 45° diamond linkages) are deployed strictly as hairline borders (1dp) or static section dividers, avoiding illustrative clutter.

## Colors

### Hierarchy & Role Distribution
The system is anchored in a native dark canvas to minimize display power consumption and eliminate eye strain during low-light study sessions.

- **Primary (`#F3D08F` - Kuning Tenun):** High-prominence actions, completion states, active tabs, and vital focus rings. Delivers strong contrast against deep indigo grounds.
- **Secondary (`#C96F4A` - Terakota):** Cultural accents, interactive tertiary states, interactive streaks, and contextual interactive cards.
- **Tertiary / Structural (`#223640` - Indigo Slate):** 1dp boundary outlines, passive icon containers, and tactile track foundations.
- **Neutral Canvas (`#0E171C` - Indigo Malam):** Base viewport canvas tone.

### Surface System
- Canvas background uses `#0E171C`.
- Elevated card and bottom bar containers shift up to `#16242B`.
- Stroke edges and structural dividers strictly resolve to `#223640`.

### Accessibility & Contrast Rules
All text elements comply with WCAG AA (minimum 4.5:1 ratio):
- Headings & Primary Body (`#F4EBDD`) on Canvas (`#0E171C`): Contrast ratio exceeds 13.5:1.
- Muted Sand (`#B8AEA0`) on Card Surface (`#16242B`): Contrast ratio exceeds 5.2:1.
- Primary CTA buttons feature a solid fill of `#F3D08F` paired exclusively with `#0E171C` typography (12.2:1 contrast).

### Semantic Linguistic Attribution
To prevent cognitive confusion between target and native languages:
- **Dawan Language Text:** Displayed in `#F4EBDD` paired with an amber diamond badge (`#F3D08F`).
- **Indonesian Language Text:** Displayed in `#B8AEA0` with a neutral slate pill container.

## Typography

### Structural Pairings
- **Display & Headings:** `Plus Jakarta Sans` provides geometric weight, clean terminals, and disciplined character balance suitable for mobile density.
- **Reading & Instructional Content:** `Inter` handles long-form explanations and vocabulary translations, maintaining high legibility on lower-density Android displays (mdpi/hdpi).

### Accessibility & Small-Screen Scaling
- Body typography enforces a hard minimum floor of 16sp on standard 360dp mobile viewports.
- Line heights follow strict 4px grid alignments (16sp text operates on a 24sp line box) to avoid character collision during Android system font scaling.
- Decorative, italicized, or condensed display fonts are excluded from pedagogical text to prevent glyph ambiguity in non-standard orthography.

## Layout & Spacing

### Mathematical Grid Model
The design system operates on a base-4 grid system (4dp, 8dp, 12dp, 16dp, 24dp, 32dp):
- **Margin:** Outer margins default to 16dp on standard mobile screens (`360dp - 412dp`), increasing to 24dp on tablets (`600dp+`).
- **Gutter:** In-grid gaps maintain 16dp to safeguard touch targets.
- **Touch Target Floor:** All interactive trigger zones strictly enforce a 48dp × 48dp minimum bounding box.

### Viewport Adaptation
- **Mobile Portrait (Default):** Single-column stacked stream. Persistent bottom bar with 4 standard tabs.
- **Mobile Landscape & Tablet:** Two-pane split canvas layout where lesson progress paths pin to the left (40% width) and pedagogical exercise mechanics dock to the right (60% width).

## Elevation & Depth

### Low-Draw-Call Tonal Hierarchy
This design system avoids box shadows, directional lighting effects, and multi-layered blurs. Visual layering is managed through flat tonal staging:

1. **Level 0 (Canvas Base):** Deep `#0E171C`. Houses full-bleed root containers and inactive structural tracks.
2. **Level 1 (Card & Module Foundation):** `#16242B` bounded by a 1dp crisp outline of `#223640`. Houses vocabulary lists, topic cards, and inactive answer choices.
3. **Level 2 (Active Focus & Modal Surface):** `#223640` bounded by `#F3D08F` (1dp). Reserved for selected answers, active cards, and modal review bottom sheets.

### Performance Bounds
- Shaders and realtime canvas blur effects are disabled.
- Dividers use 1dp lines filled with `#223640` without gradients or softening.

## Shapes

### Shape Language & Geometry
The interface combines architectural stability with ergonomic touch contours:
- **Cards & Instructional Surfaces:** Structured with `12px` to `16px` radius (`rounded-lg`). This softens containment zones without feeling overly playful.
- **Interactive Controls & Status Indicators:** Configured with full pill geometry (height 48dp with a 24dp radius), creating clear tactile affordance for touch targets.
- **Motif Geometry:** Micro-dividers incorporate subtle 45-degree diamond glyphs (`4dp x 4dp`) centered along 1dp vector tracks, echoing traditional weaving patterns without raster assets.

## Components

### 1. Buttons
- **Primary Action:** Solid fill `#F3D08F`, text `#0E171C`, font `Plus Jakarta Sans` Bold (16sp), pill shape (24dp radius), min height 48dp. Pressed state shifts fill to `#C96F4A`.
- **Secondary Action:** Transparent fill, 1.5dp border `#223640`, text `#F4EBDD`. Pressed state shifts border to `#F3D08F` and surface to `#16242B`.
- **Ghost Action:** No border, transparent background, text `#F4EBDD` with underline or `#F3D08F`. Min target 48dp × 48dp.

### 2. Lesson Cards
- **Inactive/Locked:** Background `#0E171C`, border 1dp `#223640`, text `#B8AEA0`, status icon locked at 40% opacity.
- **Active:** Background `#16242B`, border 1.5dp `#F3D08F`, text `#F4EBDD`. Includes progress track and accent status text.
- **Completed:** Background `#16242B`, border 1dp `#5FA97A`, accompanied by a green check mark.

### 3. Status & Linguistic Chips
- Height 28dp, pill shape (14dp radius), padding horizontal 12dp.
- **Offline / Outbox:** Background `#223640`, text `#B8AEA0`.
- **Dawan Language Badge:** Background `#223640`, border 1dp `#F3D08F`, text `#F3D08F`.
- **Indonesian Language Badge:** Background `#16242B`, border 1dp `#223640`, text `#B8AEA0`.

### 4. Progress Bars
- Height 8dp, pill shape. Base background `#16242B` with 1dp border `#223640`. Indicator fill `#F3D08F` or `#5FA97A` with rounded caps.

### 5. Gamification Indicators
- Inline badge group: Heart (`#C6544C`), Streak (`#E39B5C`), XP (`#F3D08F`). 
- Numerical values set in `Plus Jakarta Sans` SemiBold with minimum 8dp separation.

### 6. Interactive Answer Tiles
- Base state: Background `#16242B`, border 1dp `#223640`, min height 56dp, padding 16dp.
- Selected state: Background `#223640`, border 2dp `#F3D08F`, text `#F4EBDD`.
- Evaluation state (Correct): Background `#16242B`, border 2dp `#5FA97A`, text `#F4EBDD`.
- Evaluation state (Incorrect): Background `#16242B`, border 2dp `#C6544C`, text `#F4EBDD`.