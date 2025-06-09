// 設定 cookie
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const d = new Date();
        d.setTime(d.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + d.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// 讀取 cookie
function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(";");
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        // 去除前置空格
        while (c.charAt(0) === " ") c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
// 刪除 cookie
function deleteCookie(name) {
    // 設定 Cookie 的值為空，並將 Expires 設定為過去的日期
    document.cookie = name + "=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT;";
}

// 計算登入時長並轉換成 小時:分:秒 格式
function getLogOutTime() {
    const LogOutTime = getCookie("LogOutTime")
    if (!LogOutTime) {
        return "";
    }
    // 取得目前時間並計算差值（毫秒）
    const currentTime = Date.now()
    const timeDifference = sessionStorage.getItem("timeDifference")
    // 剩餘時間 = 登入到期時間 - (目前時間-本地與系統時間差)
    const remainTime = LogOutTime - (currentTime - timeDifference)
    // 轉換毫秒為秒、分與小時
    const totalSeconds = Math.floor(remainTime / 1000);

    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = totalSeconds % 60;
    sessionStorage.setItem("showLogOutTime", `${minutes} 分 ${seconds} 秒`)
    return `${minutes} 分 ${seconds} 秒`;
}

// 設定定時器，每秒更新頁面上的登入時長顯示
setInterval(() => {
    document.getElementById("showLogOutTime").innerText = "自動登出時間：" + getLogOutTime();
}, 1000);