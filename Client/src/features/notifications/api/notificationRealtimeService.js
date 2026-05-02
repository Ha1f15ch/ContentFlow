import * as signalR from "@microsoft/signalr";
import { getToken } from "@/shared/api/TokenStorage";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://127.0.0.1:8080/api";
const HUB_URL = API_BASE_URL.replace(/\/api\/?$/, "") + "/hubs/notifications";

let connection = null;

export async function startNotificationConnection({ onNotification } = {}) {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    return connection;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
      accessTokenFactory: () => getToken() ?? "",
    })
    .withAutomaticReconnect()
    .build();

  const handleNotification = (type) => (payload) => {
    onNotification?.({ type, payload });
  };

  connection.on("NewSubscriber", handleNotification("NewSubscriber"));
  connection.on("PostPublished", handleNotification("PostPublished"));

  await connection.start();
  return connection;
}

export async function stopNotificationConnection() {
  if (!connection) return;

  await connection.stop();
  connection = null;
}
