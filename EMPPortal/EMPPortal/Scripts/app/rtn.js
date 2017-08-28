function checkABA(s) {
    var i, n, t;
    // First, remove any non-numeric characters.
    t = "";
    for (i = 0; i < s.length; i++) {
        c = parseInt(s.charAt(i), 10);
        if (c >= 0 && c <= 9)
            t = t + c;
    }

    // Check the length, it should be nine digits.

    if (t.length != 9)
        return false;

    // Now run through each digit and calculate the total.

    n = 0;
    for (i = 0; i < t.length; i += 3) {
        n += parseInt(t.charAt(i),     10) * 3
          +  parseInt(t.charAt(i + 1), 10) * 7
          +  parseInt(t.charAt(i + 2), 10);
    }

    // If the resulting sum is an even multiple of ten (but not zero),
    // the aba routing number is good.

    if (n != 0 && n % 10 == 0)
        return true;
    else
        return false;
}

function validateForm(f) {
    var s = f.elements["aba"].value;
    if (checkABA(s))
        alert("OK");
    else
        alert("INVALID");
    return false;
}

//]]></script>