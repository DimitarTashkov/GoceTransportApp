document.addEventListener("DOMContentLoaded", function () {
    var timeInput = document.getElementById("filterTime");

    if (!timeInput.value) {
        timeInput.setAttribute("placeholder", "Filter by a time");
        timeInput.type = "text";

        timeInput.addEventListener("focus", function () {
            this.type = "time";
            this.removeAttribute("placeholder");
        });

        timeInput.addEventListener("blur", function () {
            if (!this.value) {
                this.type = "text";
                this.setAttribute("placeholder", "Filter by a time");
            }
        });
    }
});