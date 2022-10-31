// jQuery(document).ready(function() {
//   jQuery(".input-value").on("click", function() {
//     jQuery(this).val("");
//   });
// });

jQuery(document).ready(function() {
  jQuery(".reset-input").on("click", function() {
    console.log("reset");
    jQuery(this)
      .closest(".input-value")
      .val("");
  });
});
