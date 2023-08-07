import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd());

    return {
        plugins: [react()],
        build: {
            outDir: "dist",
            emptyOutDir: true,
            sourcemap: true
        }
        //define: { "process.env": {} }
        // server: {
        //     proxy: {
        //         "/ask": env.VITE_API_BASE_URL,
        //         "/chat": env.VITE_API_BASE_URL
        //     }
        // }
    };
});
