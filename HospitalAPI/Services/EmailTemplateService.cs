namespace HospitalAPI.Services
{
    public static class EmailTemplateService
    {
        public static string AppointmentBooked(
            string patientName,
            string doctorName,
            string? department,
            DateTime appointmentDate,
            string status)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f4f6f9;
            padding: 30px;
        }}

        .container {{
            max-width: 650px;
            margin: auto;
            background: white;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,.1);
        }}

        .header {{
            background: #0d6efd;
            color: white;
            padding: 20px;
            text-align: center;
        }}

        .content {{
            padding: 25px;
        }}

        table {{
            width: 100%;
            border-collapse: collapse;
        }}

        td {{
            border: 1px solid #ddd;
            padding: 10px;
        }}

        .footer {{
            text-align:center;
            padding:15px;
            background:#f1f1f1;
            color:#666;
            font-size:13px;
        }}
    </style>
</head>

<body>

<div class='container'>

<div class='header'>
<h2>🏥 Hospital Management System</h2>
</div>

<div class='content'>

<h3>Hello {patientName},</h3>

<p>Your appointment has been booked successfully.</p>

<table>

<tr>
<td><strong>Doctor</strong></td>
<td>{doctorName}</td>
</tr>

<tr>
<td><strong>Department</strong></td>
<td>{department}</td>
</tr>

<tr>
<td><strong>Date</strong></td>
<td>{appointmentDate:dd MMM yyyy}</td>
</tr>

<tr>
<td><strong>Time</strong></td>
<td>{appointmentDate:hh:mm tt}</td>
</tr>

<tr>
<td><strong>Status</strong></td>
<td>{status}</td>
</tr>

</table>

<br/>

<p>
Thank you for choosing our hospital.
</p>

</div>

<div class='footer'>
© 2026 Hospital Management System
</div>

</div>

</body>
</html>";
        }


        public static string AppointmentApproved(
    string patientName,
    string doctorName,
    DateTime appointmentDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<style>
body{{font-family:Arial;background:#f5f5f5;padding:20px;}}
.container{{max-width:650px;margin:auto;background:white;border-radius:10px;overflow:hidden;}}
.header{{background:#198754;color:white;padding:20px;text-align:center;}}
.content{{padding:25px;}}
.footer{{background:#eee;padding:15px;text-align:center;font-size:12px;}}
</style>
</head>

<body>

<div class='container'>

<div class='header'>
<h2>✅ Appointment Approved</h2>
</div>

<div class='content'>

<p>Hello <strong>{patientName}</strong>,</p>

<p>Your appointment has been approved.</p>

<p><strong>Doctor:</strong> {doctorName}</p>

<p><strong>Date:</strong> {appointmentDate:dd MMM yyyy}</p>

<p><strong>Time:</strong> {appointmentDate:hh:mm tt}</p>

<p>Please arrive at least 15 minutes before your scheduled appointment.</p>

</div>

<div class='footer'>
Hospital Management System
</div>

</div>

</body>
</html>";
        }





        public static string AppointmentCancelled(
    string patientName,
    string doctorName,
    DateTime appointmentDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<style>
body{{font-family:Arial;background:#f5f5f5;padding:20px;}}
.container{{max-width:650px;margin:auto;background:white;border-radius:10px;overflow:hidden;}}
.header{{background:#dc3545;color:white;padding:20px;text-align:center;}}
.content{{padding:25px;}}
.footer{{background:#eee;padding:15px;text-align:center;font-size:12px;}}
</style>
</head>

<body>

<div class='container'>

<div class='header'>
<h2>❌ Appointment Cancelled</h2>
</div>

<div class='content'>

<p>Hello <strong>{patientName}</strong>,</p>

<p>Unfortunately, your appointment has been cancelled.</p>

<p><strong>Doctor:</strong> {doctorName}</p>

<p><strong>Date:</strong> {appointmentDate:dd MMM yyyy}</p>

<p><strong>Time:</strong> {appointmentDate:hh:mm tt}</p>

<p>Please contact the hospital if you would like to schedule another appointment.</p>

</div>

<div class='footer'>
Hospital Management System
</div>

</div>

</body>
</html>";
        }


        public static string AppointmentCompleted(
    string patientName,
    string doctorName,
    DateTime appointmentDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<style>
body{{font-family:Arial;background:#f5f5f5;padding:20px;}}
.container{{max-width:650px;margin:auto;background:white;border-radius:10px;overflow:hidden;}}
.header{{background:#0d6efd;color:white;padding:20px;text-align:center;}}
.content{{padding:25px;}}
.footer{{background:#eee;padding:15px;text-align:center;font-size:12px;}}
</style>
</head>

<body>

<div class='container'>

<div class='header'>
<h2>✅ Appointment Completed</h2>
</div>

<div class='content'>

<p>Hello <strong>{patientName}</strong>,</p>

<p>Your appointment has been completed successfully.</p>

<p><strong>Doctor:</strong> {doctorName}</p>

<p><strong>Date:</strong> {appointmentDate:dd MMM yyyy}</p>

<p>Thank you for visiting our hospital.</p>

<p>Your prescription will be available shortly if one has been issued.</p>

</div>

<div class='footer'>
Hospital Management System
</div>

</div>

</body>
</html>";
        }


        public static string PrescriptionCreated(
    string patientName,
    string doctorName,
    DateTime appointmentDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
<style>
body{{font-family:Arial;background:#f5f5f5;padding:20px;}}
.container{{max-width:650px;margin:auto;background:white;border-radius:10px;overflow:hidden;}}
.header{{background:#6f42c1;color:white;padding:20px;text-align:center;}}
.content{{padding:25px;}}
.footer{{background:#eee;padding:15px;text-align:center;font-size:12px;}}
</style>
</head>

<body>

<div class='container'>

<div class='header'>
<h2>💊 Prescription Ready</h2>
</div>

<div class='content'>

<p>Hello <strong>{patientName}</strong>,</p>

<p>Your doctor has created a prescription for your recent appointment.</p>

<p><strong>Doctor:</strong> {doctorName}</p>

<p><strong>Appointment Date:</strong> {appointmentDate:dd MMM yyyy}</p>

<p>Please log in to the Hospital Management System to view your prescription.</p>

<p>We wish you a speedy recovery.</p>

</div>

<div class='footer'>
Hospital Management System
</div>

</div>

</body>
</html>";
        }



    }
}