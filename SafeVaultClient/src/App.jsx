import { BrowserRouter as Router, Route, Routes, Link } from "react-router-dom";
import LoginForm from './Component/LoginForm';
import RegisterForm from './Component/RegisterForm';


export default function App() {
  return (
    <Router>
      <div style={{ display: 'flex', gap: '2rem', padding: '2rem' }}>
        <Link to="/">Go to Login Form</Link>
        <Link to="/register">Go to Register Form</Link>
      </div>

      <Routes>
        <Route path="/" element={<LoginForm />} />
        <Route path="/register" element={<RegisterForm />} />
      </Routes>
    </Router>
  );
}