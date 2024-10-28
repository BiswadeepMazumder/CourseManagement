import React, { useEffect, useState } from 'react';
import { Box, Table, Thead, Tbody, Tr, Th, Td, TableContainer, Heading, Spinner, Text, Button } from '@chakra-ui/react';
import { useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import axios from 'axios';

interface Course {
  courseId: number;
  courseName: string;
  description: string;
  maxSeats: number;
  currentSeats: number;
  startDate: string;
  endDate: string;
}

const CoursePage = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
// const navigate = useNavigate();  // Initialize useNavigate for navigation

  useEffect(() => {
    const fetchCourses = async () => {
      try {
        console.log("Fetching courses from API...");
        const response = await axios.get('http://localhost:5000/api/Course/ShowAllCourses');
        
        console.log("API Response:", response.data);  // Log the response data
        setCourses(response.data);
        setLoading(false);
      } catch (err) {
        console.error("Error fetching courses:", err);  // Log the error
        setError('Failed to fetch courses.');
        setLoading(false);
      }
    };

    fetchCourses();
  }, []);

  if (loading) {
    return (
      <Box textAlign="center" mt={10}>
        <Spinner size="xl" />
      </Box>
    );
  }

  if (error) {
    return (
      <Box textAlign="center" mt={10}>
        <Text color="red.500">{error}</Text>
      </Box>
    );
  }

  return (
    <Box p={8}>
      <Heading as="h2" size="xl" mb={6} textAlign="center">Available Courses</Heading>

      <TableContainer>
        <Table variant="simple">
          <Thead>
            <Tr>
              <Th>Course ID</Th>
              <Th>Course Name</Th>
              <Th>Description</Th>
              <Th>Max Seats</Th>
              <Th>Current Seats</Th>
              <Th>Start Date</Th>
              <Th>End Date</Th>
            </Tr>
          </Thead>
          <Tbody>
            {courses.map((course) => (
              <Tr key={course.courseId}>
                <Td>{course.courseId}</Td>
                <Td>{course.courseName}</Td>
                <Td>{course.description}</Td>
                <Td>{course.maxSeats}</Td>
                <Td>{course.currentSeats}</Td>
                <Td>{course.startDate}</Td>
                <Td>{course.endDate}</Td>
              </Tr>
            ))}
          </Tbody>
        </Table>
      </TableContainer>
    </Box>
  );
};

export default CoursePage;
