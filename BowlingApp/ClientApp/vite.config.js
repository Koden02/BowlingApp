import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
	plugins: [react()],
	server: {
		port: 3000,
		strictPort: true,
		proxy: {
			"/api": {
				target: "https://localhost:7156",
				changeOrigin: true,
				secure: false
			}
		}
	}
});