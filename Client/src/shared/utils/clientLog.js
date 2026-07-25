const isDev = import.meta.env.DEV;

export const clientLog = {
  debug(...args) {
    if (isDev) console.debug(...args);
  },
  info(...args) {
    if (isDev) console.info(...args);
  },
  warn(...args) {
    if (isDev) console.warn(...args);
  },
  error(...args) {
    // Keep errors visible in production for real failures.
    console.error(...args);
  },
};
