/**
 * Cortikal — Registry Type Definitions
 *
 * Types for the Architecture Registry — the community-driven
 * collection of arch.md templates.
 */

import type { ArchMetadata } from "./graph-types";

/** A registry entry representing a published arch.md template */
export interface RegistryEntry {
  /** Unique slug identifier */
  slug: string;
  /** Full metadata from the arch.md frontmatter */
  metadata: ArchMetadata;
  /** URL to the arch.md file */
  archUrl: string;
  /** URL to the preview image */
  previewUrl?: string;
  /** GitHub stars / community rating */
  stars: number;
  /** Number of times this template has been imported */
  downloads: number;
  /** Whether this is an official Cortikal template */
  isOfficial: boolean;
  /** Contributor info */
  contributor: {
    name: string;
    avatarUrl?: string;
    profileUrl?: string;
  };
}

/** Search/filter parameters for the registry */
export interface RegistrySearchParams {
  query?: string;
  tags?: string[];
  complexity?: ArchMetadata["complexity"];
  sortBy?: "stars" | "downloads" | "recent";
  page?: number;
  pageSize?: number;
}

/** Paginated registry search results */
export interface RegistrySearchResult {
  entries: RegistryEntry[];
  total: number;
  page: number;
  pageSize: number;
}
