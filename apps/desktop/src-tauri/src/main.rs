//! Cortikal Desktop Application
//!
//! Tauri v2 entry point providing secure access to the local file system,
//! terminal execution, Docker integration, and Git operations.

#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod commands;

fn main() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![
            commands::greet,
            commands::filesystem::read_file,
            commands::filesystem::write_file,
            commands::filesystem::list_directory,
        ])
        .run(tauri::generate_context!())
        .expect("error while running Cortikal Desktop");
}
