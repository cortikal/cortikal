/**
 * Cortikal — Parser Types
 */

import type { ArchDocument } from "@cortikal/shared-types";

/** Result of parsing an arch.md file */
export interface ParseResult {
  success: boolean;
  document?: ArchDocument;
  errors: ParseError[];
}

/** A parse error with location information */
export interface ParseError {
  message: string;
  line?: number;
  column?: number;
  severity: "error" | "warning";
}

/** Result of validating an ArchDocument */
export interface ValidationResult {
  valid: boolean;
  errors: ValidationError[];
  warnings: ValidationWarning[];
}

/** A validation error */
export interface ValidationError {
  path: string;
  message: string;
  code: string;
}

/** A validation warning */
export interface ValidationWarning {
  path: string;
  message: string;
  code: string;
}
