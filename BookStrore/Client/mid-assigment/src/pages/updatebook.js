import { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useParams } from "react-router-dom";
import { Select } from "antd";
import { Option } from "antd/lib/mentions";

function UpdateBook() {
  const navigate = useNavigate();

  const { id } = useParams();

  const [category, setCategory] = useState([]);

  const [data, setData] = useState([]);

  const handleBackToList = () => {
    navigate(`/book`);
  };

  const [book, setBook] = useState({
    bookName: "",
    categoryIds: [],
  });

  const handleChange = (event) => {
    setBook({
      ...book,
      [event.target.name]: event.target.value,
    });
  };
  
  const handleChange2 = (value) => {
    setBook({
      ...book,
      categoryIds: value,
    });
  };

  useEffect(() => {
    axios({
      method: "GET",
      url: `https://localhost:7233/api/category-management/categories`,
      data: null,
    })
      .then((data) => {
        console.log(data.data);
        setCategory(data.data);
      })
      .catch((err) => {
        console.log(err);
      });
  }, []);

  useEffect(() => {
    axios({
      method: "GET",
      url: `https://localhost:7233/api/book-management/books/${id}`,
      data: null,
    })
      .then((response) => {
        console.log(response.data);
        setData(response.data);
      })
      .catch((err) => {
        console.log(err);
      });
  }, []);
  
  console.log(book);
  const handleOnSubmit = (evt) => {
    evt.preventDefault();
    axios({
      method: "put",
      url: `https://localhost:7233/api/book-management/books/${data.bookId}`,
      data: {
        bookName: book.name,
        categoryIds: book.categoryIds,
      },
    })
      .then((response) => {
        console.log(response);
      })
      .catch((error) => {
        console.log(error);
      });
    handleBackToList();
  };
  return (
    <div>
      <h1>Update book {book.name}:</h1>
      <form onSubmit={handleOnSubmit}>
        <div>
          <label>
            Book name:
            <input
              type="text"
              onChange={handleChange}
              defaultValue={data.name}
              name="name"
              required
            ></input>
          </label>
        </div>

        <div>
          <label>
            Category Id:
            <Select name="categoryIds"
              mode="multiple"
              style={{
                width: "100%",
              }}
              placeholder="select category"
              onChange={handleChange2}
              optionLabelProp="label"
            >
              {category.map((option) => ( <Option value={option.categoryId} label={option.categoryName}>
                <div className="demo-option-label-item">
                  {option.categoryId}
                </div>
              </Option>))}
            </Select>
          </label>
        </div>
        <div>
          <button class="submitButton" value={data.bookId} type="submit">
            Submit
          </button>
          <button
            class="backButton"
            onClick={handleBackToList}
            variant="secondary"
            type="reset"
          >
            Back to Post Page
          </button>
        </div>
      </form>
    </div>
  );
}
export default UpdateBook;
