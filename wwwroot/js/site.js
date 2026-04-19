// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    // Add any global JavaScript here
    console.log('Site JavaScript loaded');
});

// Set user info from session
function setUserInfo() {
    var userId = sessionStorage.getItem('UserId');
    var username = sessionStorage.getItem('Username');
    var userRole = sessionStorage.getItem('Role');
    
    if (username) {
        var userElement = document.getElementById('user-info');
        if (userElement) {
            userElement.textContent = 'Chào, ' + username + ' (' + userRole + ')';
        }
    }
}

// Call function on page load
window.addEventListener('load', setUserInfo);
