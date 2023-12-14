<?php
// Добавленные строки для защиты от брутфорса
session_start();
$login_attempts_key = 'login_attempts';
$max_attempts = 5;
$lockout_duration = 300; // 5 минут

if (isset($_SESSION[$login_attempts_key]) && $_SESSION[$login_attempts_key] >= $max_attempts) {
    // Блокировка на определенное время
    $html .= "<pre><br />Too many login attempts. Please try again later.</pre>";
} elseif (isset($_GET['Login'])) {
    // Get username
    $user = $_GET['username'];
    // Get password
    $pass = $_GET['password'];
    $pass = md5($pass);
    // Check the database
    $query  = "SELECT * FROM `users` WHERE user = '$user' AND password = '$pass';";
    $result = mysqli_query($GLOBALS["___mysqli_ston"],  $query) or die('<pre>' . ((is_object($GLOBALS["___mysqli_ston"])) ? mysqli_error($GLOBALS["___mysqli_ston"]) : (($___mysqli_res = mysqli_connect_error()) ? $___mysqli_res : false)) . '</pre>');
    
    if ($result && mysqli_num_rows($result) == 1) {
        // Reset login attempts on successful login
        $_SESSION[$login_attempts_key] = 0;
        // Get users details
        $row    = mysqli_fetch_assoc($result);
        $avatar = $row["avatar"];
        // Login successful
        $html .= "<p>Welcome to the password protected area {$user}</p>";
        $html .= "<img src=\"{$avatar}\" />";
    } else {
        // Increment login attempts on failed login
        $_SESSION[$login_attempts_key] = isset($_SESSION[$login_attempts_key]) ? $_SESSION[$login_attempts_key] + 1 : 1;
        // Login failed
        $html .= "<pre><br />Username and/or password incorrect.</pre>";
    }

    ((is_null($___mysqli_res = mysqli_close($GLOBALS["___mysqli_ston"]))) ? false : $___mysqli_res);
}
?>
