import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Button, FormControl, FormLabel, Input, Stack, Heading, Flex, Text } from '@chakra-ui/react';
import axios from 'axios';

const Login = () => {
  const navigate = useNavigate();  
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const response = await axios.post('http://localhost:5000/api/Students/Login', { email, password });

      if (response.status === 200) {
        const { fullName, userId, userType } = response.data; // Ensure userType is included

        // Navigate to the dashboard and pass user information, including userType
        navigate('/dashboard', { state: { fullName, userId, userType } });
      } else {
        setErrorMessage('Invalid email or password.');
      }
    } catch (error) {
      setErrorMessage('Login failed. Please check your credentials and try again.');
    }
  };

  return (
    <Flex minHeight="100vh" align="center" justify="center" bg="gray.50">
      <Box p={8} maxWidth="400px" borderWidth={1} borderRadius={8} boxShadow="lg" bg="white">
        <Box textAlign="center" mb={6}>
          <Heading as="h2" size="lg">Welcome</Heading>
        </Box>
        <form onSubmit={handleLogin}>
          <Stack spacing={4}>
            <FormControl id="email" isRequired>
              <FormLabel>Email address</FormLabel>
              <Input
                type="email"
                placeholder="Enter your email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </FormControl>
            <FormControl id="password" isRequired>
              <FormLabel>Password</FormLabel>
              <Input
                type="password"
                placeholder="Enter your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </FormControl>
            <Button colorScheme="teal" size="lg" type="submit" width="full">
              Sign In
            </Button>
          </Stack>
        </form>
        {errorMessage && <Text color="red.500" mt={4}>{errorMessage}</Text>}
      </Box>
    </Flex>
  );
};

export default Login;
