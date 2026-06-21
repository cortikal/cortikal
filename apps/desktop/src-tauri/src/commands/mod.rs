pub mod filesystem;

/// Simple greet command for testing Tauri IPC.
#[tauri::command]
pub fn greet(name: &str) -> String {
    format!("Hello from Cortikal Desktop, {}!", name)
}
