document.addEventListener("DOMContentLoaded", function () {
    var dateInput = document.getElementById("filterDate");

    if (!dateInput.value) {
        dateInput.setAttribute("placeholder", "Filter by a date");
        dateInput.type = "text";

        dateInput.addEventListener("focus", function () {
            this.type = "date";
            this.removeAttribute("placeholder");
        });

        dateInput.addEventListener("blur", function () {
            if (!this.value) {
                this.type = "text";
                this.setAttribute("placeholder", "Filter by a date");
            }
        });
    }
});
